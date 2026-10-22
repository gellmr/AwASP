using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class BiSaleHistController : AdminBaseController
  {
    private readonly StoreContext _context;

    public BiSaleHistController(
      UserManager<AppUser> userManager,
      IGuestRepository gRepo,
      StoreContext context) : base(userManager, gRepo)
    {
      _context = context;
    }

    [HttpGet("admin-hist-sales")]
    public async Task<ActionResult> GetHistoricalSales()
    {
      try
      {
        // 1. Group by raw DateTime on the database side
        var rawData = await _context.Set<OrderedProduct>()
          .Include(op => op.Order)
          .Where(op => op.Order != null && op.Order.OrderPlacedDate != null)
          .GroupBy(op => op.Order!.OrderPlacedDate!.Value.Date)
          .Select(g => new {
            DateVal = g.Key,
            TotalSales = g.Sum(op => op.Quantity)
          })
          .OrderBy(x => x.DateVal)
          .ToListAsync();

        // 2. Format the date string safely in memory after fetching
        var salesData = rawData.Select(x => new {
          date = x.DateVal.ToString("yyyy-MM-dd"),
          totalSales = x.TotalSales
        }).ToList();

        return Ok(salesData);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching historical sales.");
      }
    }
  }
}
