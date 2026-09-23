using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.DTO.OrderDTOs;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CheckoutController: ShopController
  {
    private IOrdersRepository ordersRepo;

    public CheckoutController(ICartLineRepository rRepo, IGuestRepository gRepo, IInStockRepository pRepo, IOrdersRepository oRepo, Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager) : base(rRepo, gRepo, pRepo, userManager) {
      ordersRepo = oRepo;
    }

    [NonAction]
    protected Order PopulateOrder(CheckoutSubmitDTO checkoutSubmit)
    {
      DateTimeOffset now = DateTimeOffset.Now;

      Order order1 = new Order();
      order1.OrderPlacedDate = now;

      order1.BillAddress = new Address{
        Line1 = checkoutSubmit.ShipLine1,
        Line2 = checkoutSubmit.ShipLine2,
        Line3 = checkoutSubmit.ShipLine3,
        City = checkoutSubmit.ShipCity,
        State = checkoutSubmit.ShipState,
        Country = checkoutSubmit.ShipCountry,
        Zip = checkoutSubmit.ShipZip
      };

      order1.ShipAddress = new Address{
        Line1 = checkoutSubmit.ShipLine1,
        Line2 = checkoutSubmit.ShipLine2,
        Line3 = checkoutSubmit.ShipLine3,
        City = checkoutSubmit.ShipCity,
        State = checkoutSubmit.ShipState,
        Country = checkoutSubmit.ShipCountry,
        Zip = checkoutSubmit.ShipZip
      };
      
      order1.OrderStatus = Order.ParseShippingState(ShippingState.OrderPlaced);
      
      // Create an ordered product for each cart line
      foreach (CartSubmitLineDTO line in checkoutSubmit.cart){
        InStockProduct myIsp = (InStockProduct)inStockRepo.InStockProducts.FirstOrDefault(p => p.ID == line.isp.id);
        OrderedProduct op1 = new OrderedProduct{
          InStockProduct = myIsp,
          InStockProductID = line.isp.id,
          OrderID = order1.ID,
          Order = order1,
          Quantity = line.qty
        };
        order1.OrderedProducts.Add(op1);
      }
      return order1;
    }

    [HttpPost("submit")] // POST api/checkout/submit.  Accepts application/json POST submissions containing stringified JSON data in request body.
    public async Task<IActionResult> Submit([FromBody] CheckoutSubmitDTO checkoutSubmit)
    {
      try
      {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        Order order1 = PopulateOrder(checkoutSubmit);

        bool savedOk = false;
        AppUser? appUser;
        string? uid = GetLoggedInUserIdFromIdentityCookie(); // Lookup the currently logged in user.
        if (uid != null)
        {
          // Checkout submit by logged in AppUser
          appUser = await _userManager.FindByIdAsync(uid);
          order1.AppUser = appUser;
          order1.UserID = uid;
          savedOk = await ordersRepo.SaveOrderAsync(order1);
          if (savedOk){
            cartLineRepo.ClearUserCartLines(order1.UserID); // Clear the cart of this user.
            return Ok(checkoutSubmit); // Respond with 200 OK, and automatically cast object to JSON for the response.
          }
        }
        else
        {
          // Checkout submit by Guest

          // Save the submitted name and email values, to the guest record in database.
          Guest guest = await EnsureGuestFromCookieAndDb(new Guest{
            Email     = checkoutSubmit.ShipEmail,
            FirstName = checkoutSubmit.FirstName,
            LastName  = checkoutSubmit.LastName,
          });
          if (guest == null){
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "Guest is null" });
          }
          order1.GuestID = guest.ID;
          order1.Guest = guest;
          savedOk = await ordersRepo.SaveOrderAsync(order1);
          if (savedOk){
            cartLineRepo.ClearCartLines(guest.ID); // Clear the cart of this guest.
            return Ok(checkoutSubmit); // Respond with 200 OK, and automatically cast object to JSON for the response.
          }
        }
        return this.StatusCode(StatusCodes.Status500InternalServerError, "Error while saving Order.");
      }
      catch (Exception ex){
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
