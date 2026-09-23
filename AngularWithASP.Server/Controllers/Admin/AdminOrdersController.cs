using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Controllers.Admin;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Domain.StoredProc;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class AdminOrdersController : AdminBaseController
  {
    private IOrdersRepository orderRepo;

    public AdminOrdersController(IOrdersRepository oRepo, UserManager<AppUser> userManager, IGuestRepository gRepo) : base(userManager, gRepo){
      orderRepo = oRepo;
    }

    [HttpGet("admin-orders/{pageNum}")]    // GET "/api/admin-orders"
    public async Task<IActionResult> GetOrders(Int32 pageNum = 1, string? bs = null, Int32 ps = 12)
    {
      string error = string.Empty;
      try
      {
        if (bs != null){ bs = bs.Trim(); }
        if (!(PcreValidation.ValidString(bs, MyRegex.BacklogSearchOkayRegex))){
          return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid search string");
        }
        IEnumerable<AdminOrderRow> rows = await orderRepo.GetOrdersWithUsersAsync(bs, pageNum, ps);
        if (rows == null || !rows.Any())
        {
          // No results
          return Ok(new { orders = new List<OrderSlugDTO>() });
        }
        else
        {
          // Apply sorting here, according to what the user wants.
          List<OrderSlugDTO> sorted = rows
            .OrderBy(o => o.OrderPlaced).Reverse()
            .Select(order => order.OrderSlug)
            .ToList();
          bool success = true;
          if (success){
            return Ok(new { orders = sorted }); // Automatically cast object to JSON.
          }
        }
      }
      catch(Exception ex){
        error = ex.Message;
      }
      return BadRequest(new { errMessage = "Something went wrong. " + error });
    }
  }
}
