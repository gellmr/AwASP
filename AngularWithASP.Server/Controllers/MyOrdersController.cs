using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using AngularWithASP.Server.DTO.MyOrders;

namespace AngularWithASP.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MyOrdersController: ShopController
  {
    private IOrdersRepository ordersRepo;
    
    public MyOrdersController(ICartLineRepository rRepo, IGuestRepository gRepo, IInStockRepository pRepo, IOrdersRepository oRepo, UserManager<AppUser> userManager) : base(rRepo, gRepo, pRepo, userManager){
      ordersRepo = oRepo;
    }

    [HttpPost]
    [Route("fetch-order")]
    public async Task<ActionResult> FetchOrder([FromBody] OrderIdRequest request)
    {
      try
      {
        Order order = ordersRepo.GetOrderById(request.orderID); /// Load Guest if present so the fullname will be available.
        // Also load associated Guest / AppUser if available.
        if (order.GuestID != null){
          order.Guest = guestRepo.Guests.FirstOrDefault(g => g.ID.Equals(order.GuestID));
        }
        else if (order.UserID != null){
          order.AppUser = _userManager.Users.FirstOrDefault(user => user.Id == order.UserID);
        }
        return Ok(new { Order=order, Message="Success" });
      }
      catch(Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }

    [HttpPost]
    [Route("fetch-orders")]
    public async Task<ActionResult> FetchOrders(UserIdDTO userInfo)
    {
      try
      {

        if (userInfo.uid != null){
          if (!(PcreValidation.ValidString(userInfo.uid, MyRegex.AppUserOrGuestId) || PcreValidation.ValidString(userInfo.uid, MyRegex.GoogleSubject))){
            return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid uid");
          }
        }
        if (userInfo.gid != null){
          if ( !(PcreValidation.ValidString(userInfo.gid.ToString(), MyRegex.AppUserOrGuestId) )){
            return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid gid");
          }
        }

        if (userInfo.uid == null && userInfo.gid == null){
          return this.StatusCode(StatusCodes.Status400BadRequest, "User ID unavailable.");
        }

        // Look up associated records, to display on the My Orders page.
        IList<Order> orders = ordersRepo.GetMyOrders( userInfo.uid, userInfo.gid ).ToList();

        Guest g = (userInfo.gid == null) ? null : guestRepo.Guests.FirstOrDefault(g => g.ID.Equals(userInfo.gid));
        AppUser u = (userInfo.uid == null) ? null : await _userManager.FindByIdAsync(userInfo.uid);
        foreach (Order order in orders)
        {
          if (order.GuestID != null){
            order.Guest = g;
          }
          if (order.UserID != null){
            order.AppUser = u;
          }
        }
        return Ok(new{ Rows = orders, Message = "Success"});
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
