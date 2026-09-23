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

    [HttpGet("admin-products")]
    public ActionResult GetProducts()
    {
      try
      {
        IEnumerable<InStockProduct> prods = prodRepo.InStockProducts.ToList();
        return Ok(prods);
      }
      catch (Exception ex){
        return this.StatusCode(StatusCodes.Status400BadRequest, ex.Message);
      }
    }
  }
}
