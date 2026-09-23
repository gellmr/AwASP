using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using System.Security.Claims;

namespace AngularWithASP.Server.Controllers
{
  public abstract class LoginController: ShopController
  {
    protected SignInManager<AppUser> _signInManager;
    protected IConfiguration _config;
    protected bool PersistAfterBrowserClose = false; // false means the cookie is session-based
    protected bool LockoutOnFailure = false;

    public LoginController(
      ICartLineRepository cartRepo,
      IGuestRepository guestRepo,
      IInStockRepository inStockRepo,
      IConfiguration config,
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager
    ): base(cartRepo, guestRepo, inStockRepo, userManager)
    {
      _config = config;
      _signInManager = signInManager;
    }

    // Generate a response indicating the user has successfully logged in
    [NonAction]
    protected async Task<IActionResult> LoginSuccessResponse(AppUser? appUser, Guid? guestId, bool isGoogle=false)
    {
      if (guestId != null && appUser != null) {
        var orderRepo = HttpContext.RequestServices.GetRequiredService<IOrdersRepository>();
        await cartLineRepo.MergeGuestCartIntoUserAsync(guestId.Value, appUser.Id);
        await orderRepo.MergeGuestOrdersIntoUserAsync(guestId.Value, appUser.Id);
      }
      else if (guestId != null) {
        cartLineRepo.ClearCartLines(guestId);
      }
      DeleteGuestCookie();

      return Ok(new
      {
        loginResult = "Success",
        loginType = "User",
        isGoogleSignIn = isGoogle, 
        appUserId = appUser.Id,
        fullname = appUser.FullName,
        firstname = AppUser.GetFirstName(appUser.FullName),
        lastname = AppUser.GetLastName(appUser.FullName),
        email = appUser.Email,
      });
    }
  }
}
