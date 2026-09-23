using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Infrastructure;
using AngularWithASP.Server.DTO.AdminUserAccounts;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.DTO;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class AdminUserAccountsController : AdminBaseController
  {
    protected RandomUserMeApiClient _userMeService;
    protected IHostEnvironment _hostingEnvironment;
    private IConfiguration _config;

    public AdminUserAccountsController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager, RandomUserMeApiClient userMeService, IHostEnvironment hostingEnv, IConfiguration config, IGuestRepository gRepo) : base (userManager, gRepo)
    {
      _userMeService = userMeService;
      _hostingEnvironment = hostingEnv;
      _config = config;
    }

    [HttpPost]
    [Route("admin-guest-update")]
    public ActionResult UpdateGuest([FromBody] UserDTO userUpdate)
    {
      try
      {
        // Look up Guest, update name and email, save
        _guestRepo.UpdateWithTransaction(new GuestUpdateDTO{
          ID        = (Guid)userUpdate.GuestID,
          Email     = userUpdate.Email,
          FirstName = MyExtensions.GetFirstName(userUpdate.FullName),
          LastName  = MyExtensions.getLastName(userUpdate.FullName),
          Picture   = userUpdate.Picture,
        });
        return this.StatusCode(StatusCodes.Status200OK, new { Message = "Success updating guest", Persist = userUpdate });
      }
      catch (GuestUpdateException ex)
      {
        UserDTO? revert = UserDTO.TryParse(ex.Original);
        return this.StatusCode(StatusCodes.Status400BadRequest, new { Message = ex.Message, Revert = revert });
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, new { Message = ex.Message });
      }
    }

    [HttpPost]
    [Route("admin-user-update")]
    public async Task<ActionResult> UpdateUser([FromBody] UserDTO userUpdate)
    {
      UserDTO? revert = null;
      try
      {
        // Update user
        AppUser appUser = await _userManager.FindByIdAsync(userUpdate.Id);
        revert = UserDTO.TryParse(appUser);
        appUser.updateFullName(userUpdate.FullName);
        appUser.PhoneNumber = userUpdate.PhoneNumber;
        appUser.Email = userUpdate.Email;
        var result = await _userManager.UpdateAsync(appUser);
        if (!result.Succeeded){
          throw new ArgumentException("Could not save appUser. errors: " + result.Errors.ToString());
        }
        return this.StatusCode(StatusCodes.Status200OK, new { Message = "Success updating user", Persist = userUpdate });
      }
      catch (Exception ex){
        return this.StatusCode(StatusCodes.Status400BadRequest, new { Message = ex.Message, Revert = revert });
      }
    }

    [HttpGet("admin-useraccounts")]
    public ActionResult GetUserAccounts()
    {
      try
      {
        // Grab the current user id from claims of the currently logged in user.
        string currentUserId = (User.Identity.IsAuthenticated) ? User.FindFirstValue(ClaimTypes.NameIdentifier) : string.Empty;

        AppUser? curr = _userManager.Users.FirstOrDefault(user => user.Id == currentUserId);
        UserDTO currentUser = UserDTO.TryParse(curr);

        List<UserDTO> appUsers = _userManager.Users
          .Where(u => u.Id != currentUserId)
          .OrderBy(user => user.PhoneNumber) // The rest of the list is sorted by phone number.
          .Select( u => UserDTO.TryParse(u))
          .ToList();

        List<Guest> dguests = _guestRepo.Guests.Where(g =>
          !string.IsNullOrEmpty(g.Email) &&
          !string.IsNullOrEmpty(g.FirstName)
        ).ToList();

        List<UserDTO> guests = dguests.Select(u => new UserDTO{
          Email = u.Email,
          GuestID = u.ID,
          Id = null,
          Picture = u.Picture,
          UserName = MyExtensions.GenUserName(u.FullName, u.ID.ToString().ToLower()),
          FullName = u.FullName
        })
        .OrderBy(u => u.FullName)
        .ToList();

        List<UserDTO> allUsers = [currentUser, // Ensure current user appears at top
          .. guests,   // Show guests above users
          .. appUsers, // Show users
        ];

        return Ok(allUsers);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }

    [HttpGet("admin-guest-edit/{idval}")]
    public ActionResult GetGuestAccount(string? idval)
    {
      try
      {
        // Ensure the given gid string is equivalent to a Guid
        if (!PcreValidation.ValidString(idval, MyRegex.AppUserOrGuestId)){
          return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid gid");
        }
        idval = (idval == null) ? null : idval.ToLower();
        Guest? g = _guestRepo.Guests.FirstOrDefault(g => g.ID.ToString().ToLower().Equals(idval));
        UserDTO user = UserDTO.TryParse(g);
        return Ok(user);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }

    [HttpGet("admin-user-edit/{idval}")]
    public async Task<ActionResult> GetUserAccount(string? idval)
    {
      try
      {
        // Ensure the given uid is either an AppUserId (Guid) or a Google Subject Id (20-255 numeric value)
        if ( !(PcreValidation.ValidString(idval, MyRegex.AppUserOrGuestId) || PcreValidation.ValidString(idval, MyRegex.GoogleSubject))){
          return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid uid");
        }
        AppUser? u = await _userManager.FindByIdAsync(idval);
        UserDTO user = UserDTO.TryParse(u);
        return Ok(user);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }

    [HttpPost("admin-userpic")]
    [DisableRequestSizeLimit] // Optional: disables the default file size limit
    public async Task<ActionResult> PostUserImage(IFormFile file, string? idval, string? usertype)
    {
      try
      {
        if (!(PcreValidation.ValidString(idval, MyRegex.AppUserOrGuestId) || PcreValidation.ValidString(idval, MyRegex.GoogleSubject))){
          return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid idval");
        }
        if (file == null || file.Length == 0){
          return this.StatusCode(StatusCodes.Status400BadRequest, "No file was uploaded");
        }

        // In a GCP Cloud Run environment, we cannot save to the local file system
        // because the container disk is ephemeral. We must upload to Google Cloud Storage.
        string bucketName = _config["GCP:StorageBucketName"];
        var ext = Path.GetExtension(file.FileName);
        var uniqueFileName = "userpic_" + Guid.NewGuid().ToString() + ext;
        string publicUrl = string.Empty;
        string debugStr = string.Empty;

        // If bucket name is not provided, fallback to the local file system (e.g. local dev)
        if (string.IsNullOrEmpty(bucketName))
        {
          bool isDev = _hostingEnvironment.EnvironmentName.Equals("Development");
          string UploadProfilePic = _config.GetSection("UploadProfilePic").Value;
          string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadProfilePic);
          
          if (isDev){
            uploadsFolder = Path.Combine(Directory.GetParent(_hostingEnvironment.ContentRootPath).FullName, UploadProfilePic);
          }

          if (!Directory.Exists(uploadsFolder)){
            Directory.CreateDirectory(uploadsFolder);
          }
          
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          // Save to file system
          using (var stream = new FileStream(filePath, FileMode.Create)){
            await file.CopyToAsync(stream);
          }
          
          publicUrl = "/userpic/" + uniqueFileName; // Relative to SPA root.
          debugStr = uploadsFolder;
        }
        else 
        {
          // We are in GCP Production! Upload to Cloud Storage Bucket
          using (var memoryStream = new MemoryStream()) 
          {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            // Create a Google Cloud Storage client
            var storage = await Google.Cloud.Storage.V1.StorageClient.CreateAsync();
            
            // Upload the object
            await storage.UploadObjectAsync(
                bucketName, 
                $"userpic/{uniqueFileName}", // The path/name in the bucket
                file.ContentType, 
                memoryStream
            );
          }
          
          // Construct the public absolute URL so the React frontend can render it
          publicUrl = $"https://storage.googleapis.com/{bucketName}/userpic/{uniqueFileName}";
          debugStr = $"GCP Bucket: {bucketName}";
        }

        // Get the user or guest that we are updating...
        string? idsave = null;
        if (!string.IsNullOrEmpty(usertype) && usertype == "guest")
        {
          // Look up Guest, update picture, save
          Guid gid = Guid.Parse(idval);
          await _guestRepo.UpdateWithTransaction(new GuestUpdateDTO{ ID = gid, Picture = publicUrl });
          idsave = gid.ToString().ToLower();
        }
        else
        {
          AppUser userToSave = await _userManager.FindByIdAsync(idval);
          userToSave.Picture = publicUrl;
          var result = await _userManager.UpdateAsync(userToSave);
          if (!result.Succeeded){
            throw new Exception("Could not save user. " + result.Errors.First().Description);
          }
          idsave = userToSave.Id;
        }

        // Respond with JSON including URL for the uploaded file.
        return this.StatusCode(StatusCodes.Status200OK, new {
          Message = "File uploaded successfully",
          Picture = publicUrl,
          idsave = idsave, // Id of the user or guest to save on client.
          debug = debugStr
        });
      }
      catch (GuestUpdateException ex)
      {
        UserDTO? revert = UserDTO.TryParse(ex.Original);
        return this.StatusCode(StatusCodes.Status400BadRequest, new { Message = ex.Message, Revert = revert });
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }
  }
}
