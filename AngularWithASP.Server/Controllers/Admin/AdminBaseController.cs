using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using AngularWithASP.Server.Domain.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AngularWithASP.Server.Controllers.Admin
{
  [ApiController]
  [Route("api")]
  public class AdminBaseController : MyAuthenticatedController
  {
    public AdminBaseController(UserManager<AppUser> userManager, IGuestRepository gRepo) : base(userManager, gRepo) {}
  }
}
