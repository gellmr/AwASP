using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace AngularWithASP.Server.Controllers.Admin
{
  public class AdminProductsController : AdminBaseController
  {
    private IInStockRepository prodRepo;

    public AdminProductsController(IInStockRepository pRepo, UserManager<AppUser> userManager, IGuestRepository gRepo) : base(userManager, gRepo){
      prodRepo = pRepo;
    }

    [HttpGet("admin-products/{page:int?}")]
    public ActionResult GetProducts(int page = 1, [FromQuery] int ps = 10, [FromQuery] string? bs = null)
    {
      try
      {
        if (page < 1) page = 1;
        if (ps < 1) ps = 10;

        var query = prodRepo.InStockProducts;
        
        // Apply search filter if search term is provided
        if (!string.IsNullOrEmpty(bs)){
          string term = bs.Trim().ToLower();
          query = query.Where(u => 
            (u.Description != null && u.Description.ToLower().Contains(term)) ||
            (u.Title       != null &&       u.Title.ToLower().Contains(term))
          );
        }

        int totalRows = query.Count();

        // Apply pagination slice and execute the query at the very end
        var pagedProds = query
          .Skip((page - 1) * ps)
          .Take(ps)
          .ToList();

        return Ok(new { prods = pagedProds, totalRows = totalRows });
      }
      catch (Exception ex){
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
