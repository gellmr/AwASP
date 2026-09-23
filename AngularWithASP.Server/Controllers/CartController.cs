using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CartController: ShopController
  {
    public CartController(ICartLineRepository rRepo, IGuestRepository gRepo, IInStockRepository pRepo, Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager) : base(rRepo, gRepo, pRepo, userManager){}

    [HttpGet] // GET api/cart
    public async Task<ActionResult> Get()
    {
      Guest? guest = await EnsureGuestFromCookieAndDb(null);
      if (guest == null){
        return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "Guest is null" });
      }

      string? uid = GetLoggedInUserIdFromIdentityCookie();

      // Look up cart lines for this user.
      IEnumerable<CartLine> cartLines;
      if (string.IsNullOrEmpty(uid)){
        cartLines = cartLineRepo.CartLines.Where(line => line.GuestID == guest.ID);
      }else{
        cartLines = cartLineRepo.CartLines.Where(line => line.UserID == uid);
      }

      // Load the ISP's
      List<CartUpdateDTO> cartLinesDistinctByIsp = cartLineRepo.WithIsps(cartLines)
      .DistinctBy(line => line.InStockProductID)
      .Select(cartLine => new CartUpdateDTO{
        cartLineID = (Int32)cartLine.ID,
        qty        = (Int32)cartLine.Quantity,
        isp = new IspDTO{
          id          = cartLine.InStockProduct.ID,
          title       = cartLine.InStockProduct.Title,
          description = cartLine.InStockProduct.Description,
          price       = cartLine.InStockProduct.Price,
          category    = (Int32)cartLine.InStockProduct.Category,
          image       = cartLine.InStockProduct.Image
        }
      }).ToList();

      return Ok( cartLinesDistinctByIsp ); // Respond with 200 OK, and list of cartLine objects.
    }

    [HttpPost]
    [Route("clear")] // POST api/cart/clear
    public async Task<ActionResult> Clear()
    {
      // Try to look up the guest. If no guest, create new guest.
      Guest? guest = await EnsureGuestFromCookieAndDb(null);
      if ( guest == null ){
        return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "Guest is null" });
      }

      Guid? gid = guest.ID;
      cartLineRepo.ClearCartLines(gid); // Remove all CartLine records for this Guest ID.
      return Ok(); // 200 ok
    }

    [HttpPost]
    [Route("update")] // POST api/cart/update  Accepts POST data with JSON in Request Body. Content-Type must be 'application/json'
    public async Task<ActionResult> Update([FromBody] CartUpdateDTO cartUpdate, Nullable<Guid> guestId)
    {
      // Client cart has been updated with the given quantities.
      // Update the user's cart in the database...

      try
      {
        Guest? guest = null;
        AppUser? user = null;
        string? uid = GetLoggedInUserIdFromIdentityCookie(); // Try to get ID of currently logged in user.
        if (uid != null)
        {
          user = await _userManager.FindByIdAsync(uid);
          guestId = null; // Dont use the guest id.
        }
        else
        {
          // There is no logged in user. Try to look up the guest. If no guest, create new guest.
          guest = await EnsureGuestFromCookieAndDb(null);
          if (guest == null){
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "Guest is null" });
          }
          guestId = guest.ID;
        }
      
        // Look up (isp) product in database.
        InStockProduct isp = inStockRepo.InStockProducts.FirstOrDefault(record => record.ID == cartUpdate.isp.id);
        if (isp == null){
          Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
          cartLineRepo.RemoveById((Int32)cartUpdate.cartLineID); // Remove the offending cart row from database.
          return new JsonResult(new {
            message = "InStockProduct not found",
            error = "ispRemove",
            ispRemove = cartUpdate.isp.id
          });
        }

        // Create database entry for new CartLine, connected to user/guest and the existing InStockProduct.
        CartLine cartLine = new CartLine{
          ID = cartUpdate.cartLineID ?? null, // Must be null when we are creating or DB will complain.
          GuestID = guestId,
          Guest = guest,
          InStockProductID = isp.ID,
          InStockProduct = isp,
          Quantity = cartUpdate.qty,
          UserID = uid,
          AppUser = user,
        };
        CartLine? updatedCartLine = cartLineRepo.SaveCartLine(cartLine);

        // Prepare JSON for client
        cartUpdate.cartLineID = cartUpdate.cartLineID ?? (Int32)updatedCartLine.ID;
        cartUpdate.isp = (updatedCartLine == null) ? null : new IspDTO
        {
           id          = updatedCartLine.InStockProduct.ID,
           title       = updatedCartLine.InStockProduct.Title,
           description = updatedCartLine.InStockProduct.Description,
           price       = updatedCartLine.InStockProduct.Price,
           category    = (Int32)updatedCartLine.InStockProduct.Category,
           image       = updatedCartLine.InStockProduct.Image
        };

        // Send back response to client indicating success or failure.
        return Ok(cartUpdate); // Respond with 200 OK, and the finalised cart state.
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }
  }
}
