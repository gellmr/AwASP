using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using AngularWithASP.Server.Domain.Abstract;
using Microsoft.AspNetCore.Authorization;

namespace AngularWithASP.Server.Controllers
{
  [Authorize]
  public class MyAuthenticatedController : MyBaseController
  {
    protected UserManager<AppUser> _userManager;
    protected IGuestRepository _guestRepo;

    public MyAuthenticatedController(UserManager<AppUser> userManager, IGuestRepository gRepo)
    {
      _userManager = userManager;
      _guestRepo = gRepo;
    }
  }
}
