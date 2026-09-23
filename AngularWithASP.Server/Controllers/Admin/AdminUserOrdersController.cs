using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class AdminUserOrdersController : AdminBaseController
  {
    private IOrdersRepository orderRepo;

    public AdminUserOrdersController(Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager, IGuestRepository gRepo, IOrdersRepository oRepo) : base(userManager, gRepo){
      orderRepo = oRepo;
    }

    [HttpGet("admin-user-orders")]
    public async Task<ActionResult> GetUserOrders(string? idval, string? usertype)
    {
      try
      {
        // Ensure the given idval is equivalent to a (Guid) or a Google Subject Id (20-255 numeric value)
        if (!(PcreValidation.ValidString(idval, MyRegex.AppUserOrGuestId) || PcreValidation.ValidString(idval, MyRegex.GoogleSubject))){
          return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid idval");
        }
        idval = (idval == null) ? null : idval.ToLower();
        IEnumerable<Order> orders = orderRepo.GetUserOrders(idval, usertype);

        string? fullname = string.Empty;
        if (usertype.Equals("guest"))
        {
          Guest? guest = _guestRepo.Guests.FirstOrDefault(g => g.ID.Equals( Guid.Parse(idval) ));
          fullname = guest.FullName;
        }
        else
        {
          AppUser? user = await _userManager.FindByIdAsync(idval);
          fullname = user.FullName;
        }

        return Ok(new{ Orders=orders, FullName=fullname});
      }
      catch (Exception ex){
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
