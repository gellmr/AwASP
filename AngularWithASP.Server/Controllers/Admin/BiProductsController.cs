using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class BiProductsController : AdminBaseController
  {
    private IInStockRepository prodRepo;
    private StoreContext _context; // Assuming you have your EF DbContext accessible or injected

    public BiProductsController(
      IInStockRepository pRepo,
      UserManager<AppUser> userManager,
      IGuestRepository gRepo,
      StoreContext context
    ) : base(userManager, gRepo){
      prodRepo = pRepo;
      _context = context;
    }

    [HttpGet("admin-sales-bi")]
    public async Task<ActionResult> GetSalesBiData()
    {
      try
      {
        var twelveMonthsAgo = DateTimeOffset.UtcNow.AddMonths(-12);
        var salesData = await _context.Set<OrderedProduct>()
        .Include(op => op.InStockProduct)
        .Include(op => op.Order)
        .Where(op => op.Order != null && op.Order.OrderPlacedDate >= twelveMonthsAgo)
        .GroupBy(op => op.InStockProduct.Title)
        .Select(g => new
        {
          productTitle = g.Key,
          quantitySold = g.Sum(op => op.Quantity),
          totalRevenue = g.Sum(op => op.Quantity * op.InStockProduct.Price)
        })
        .OrderByDescending(x => x.totalRevenue)
        .ToListAsync();
       return Ok(salesData);
      }
      catch (Exception ex)
      {
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
