using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.Domain.Abstract;

using Google.Apis.Auth;
using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AngularWithASP.Server.Controllers
{
  [ApiController]
  [Route("api")]
  public class GoogleTokenValidateController : LoginController
  {
    private readonly string? _clientId;
    protected string ActualUserName;

    public GoogleTokenValidateController(
      ICartLineRepository cartRepo,
      IGuestRepository guestRepo,
      IInStockRepository inStockRepo,
      IConfiguration config,
      Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager
    ) : base(cartRepo, guestRepo, inStockRepo, config, userManager, signInManager)
    {
      _clientId = config.GetSection("Authentication:Google:ClientId").Value;
    }

    // Handle collisions of non unique UserName values
    protected async Task<string> GenerateUniqueUsernameAsync(string proposedUsername)
    {
      string username = proposedUsername.Replace(" ", string.Empty);
      string finalUsername = username;
      int counter = 0;
      while (await _userManager.FindByNameAsync(finalUsername) != null){
        counter++;
        finalUsername = $"{username}{counter}";
      }
      return finalUsername;
    }

    [NonAction]
    protected async Task<GoogleAppUserDTO> GetUserDtoFromPayload(GoogleJsonWebSignature.Payload? payload)
    {
      // The payload has a value... We can now use the access token (payload) to make API calls to Google, on the users behalf.
      // Now that we already have the access token (payload) we dont need to use ExternalLoginInfo or GetExternalLoginInfoAsync

      string desiredUserName = MyExtensions.GenUserName(payload.Name, payload.Subject); // eg "diana-walters-e35"
      ActualUserName = await GenerateUniqueUsernameAsync(desiredUserName);       // eg "diana-walters-e351"

      // Token is valid, extract user information
      GoogleAppUserDTO googleAppUser = new GoogleAppUserDTO
      {
        Subject = payload.Subject, // This is the unique Google user ID. String about 21 characters long.
        Email = payload.Email,
        Picture = payload.Picture,
        UserName = ActualUserName,       // "diana-walters-e35"
        FullName = payload.Name,         // "Diana Walters"
        GivenName = payload.GivenName,   // "Diana"
        FamilyName = payload.FamilyName, // "Walters"
      };
      return googleAppUser;
    }

    [NonAction]
    protected async Task<AppUser> CreateGoogleSignInUser(GoogleAppUserDTO? googleAppUser)
    {
      return new AppUser
      {
        EmailConfirmed = true, // The user cannot sign in unless this is true.
        AccessFailedCount = 0,
        LockoutEnabled = false,
        TwoFactorEnabled = false,
        UserName = ActualUserName,
        FullName = googleAppUser.FullName,
        Email = googleAppUser.Email,
        Id = googleAppUser.Subject,
        Picture = googleAppUser.Picture
      };
    }

    [HttpPost("validate-google-token")] // POST /api/validate-google-token.  Accepts application/json POST submissions containing stringified JSON data in request body.
    public async Task<IActionResult> ValidateGoogleToken([FromBody] ConfirmGoogleAuthDTO tokenDTO)
    {
      try
      {
        // This action will be hit after Google authentication is complete.
        if (!ModelState.IsValid) {
          return BadRequest(ModelState);
        }
        Guest? guest = null;
        Guid? guestId = null;
        guest = await EnsureGuestFromCookieAndDb(null); // Touch cookie to ensure it exists.
        if (guest == null){
          return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "Guest is null" });
        }
        guestId = guest.ID;
        AppUser? appUser;
        string? uid = GetLoggedInUserIdFromIdentityCookie(); // See if there is a logged in user.

        // Exchange the authorization code (tokenDTO.credential) for an access token (payload)
        GoogleJsonWebSignature.Payload? payload = await ValidateTokenAsync(tokenDTO);
        if(payload == null){
          return this.StatusCode(StatusCodes.Status401Unauthorized, new { loginResult = "Failed", message = "Could not validate Google Login token." });
        }

        // -----------------
        // Credentials ok...

        if (uid != null){
          // Already logged in
          appUser = await _userManager.FindByIdAsync(uid);
          return await LoginSuccessResponse(appUser, guestId); // Tell the user they are already logged in.
        }

        // Not logged in yet...
        GoogleAppUserDTO googleAppUser = await GetUserDtoFromPayload(payload);
        appUser = await _userManager.FindByIdAsync(googleAppUser.Subject); // See if the user already exists in our database...

        if (appUser == null)
        {
          // There is no database record.
          // Create a new AppUser record in the database for this GoogleSignIn user.
          appUser = await CreateGoogleSignInUser(googleAppUser);
          var createResult = await _userManager.CreateAsync(appUser);
          if (!createResult.Succeeded){
            return this.StatusCode(StatusCodes.Status500InternalServerError, "Could not create AppUser record in our database for this GoogleSignIn.");
          }
          // Success creating appUser
        }
        else
        {
          // Already exits. Update our database with values from the latest google payload.
          appUser.Email = googleAppUser.Email;
          appUser.UserName = googleAppUser.UserName;
          appUser.Picture = appUser.Picture ?? googleAppUser.Picture; // Dont update Picture if we already have a value.
          await _userManager.UpdateAsync(appUser); // If the user has changed their name, email, or picture via Google, we save the new value here to our database.
        }

        // -----------------------------
        // appUser exists in database...

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, googleAppUser.Subject));
        identity.AddClaim(new Claim(ClaimTypes.Name, googleAppUser.UserName));

        await _signInManager.SignInAsync(appUser, isPersistent: PersistAfterBrowserClose); // Perform the login.

        cartLineRepo.ClearCartLines(guestId);
        return await LoginSuccessResponse(appUser, guestId, true); // Tell the user they are now logged in.
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }

    private async Task<GoogleJsonWebSignature.Payload?> ValidateTokenAsync(ConfirmGoogleAuthDTO tokenDTO)
    {
      try
      {
        GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings() {
          Audience = new List<string> { _clientId }
        };
        // The SignedToken() method within expects signedToken.Split('.') to produce 3 parts.
        // (Header, Payload, Signature) ...It throws if they are not found.
        string signedToken = tokenDTO.credential;
        GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(signedToken, settings);
        return payload;
      }
      catch (InvalidJwtException)
      { 
        return null; // Token is not a valid JWT or has expired
      }
      catch (Exception)
      {
        return null; // Handle other exceptions
      }
    }
  }
}
