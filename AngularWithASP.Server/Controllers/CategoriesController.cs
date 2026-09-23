using Microsoft.AspNetCore.Mvc;

namespace AngularWithASP.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CategoriesController: ControllerBase
  {

    private static readonly IEnumerable<ProductCategory> Cats = new List<ProductCategory>
    {
      new ProductCategory { ID=1, Title="Soccer", Segment="soccer"},
      new ProductCategory { ID=2, Title="Chess", Segment="chess"},
      new ProductCategory { ID=3, Title="Water Sport", Segment="waterSport"}
    };

    [HttpGet] // GET api/categories
    public IEnumerable<ProductCategory> GetCategories()
    {
      return Cats.ToArray();
    }
  }
}
