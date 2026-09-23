using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;

namespace AngularWithASP.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    private IInStockRepository inStockRepo;

    public ProductsController(IInStockRepository pRepo){
      inStockRepo = pRepo;
    }

    private IQueryable<InStockProduct> Prods {
      get {
        return inStockRepo.InStockProducts.AsQueryable();
      }
    }

    [HttpGet] // GET api/products
    [HttpGet("category/{category:alpha}")] // GET api/products/category/{category}?search=white
    public IEnumerable<InStockProduct> Get(string? category, [FromQuery] string? search)
    {
      bool useCategory = !String.IsNullOrEmpty(category);                          // true if we have a category
      bool useSearch = !String.IsNullOrEmpty(search);                              // true if we have a search string
      Cat paramCat = useCategory ? ProductCategory.ParseCat(category) : Cat.none;  // NOTE, parse is case sensitive
      string? searchLow = useSearch ? search.ToLower() : null;                     // convert strings to lower case for comparison.

      if (useCategory && useSearch)
      {
        // case 1: We have both category and search string
        return Prods.Where(p => p.Category.Equals(paramCat) &&
          (p.Title.ToLower().Contains(searchLow) || p.Description.ToLower().Contains(searchLow))
        ).ToList();
      }
      if (useCategory && !useSearch)
      {
        // case 2: We have category, but no search string
        return Prods.Where(p => p.Category.Equals(paramCat)).ToList();
      }
      if (!useCategory && useSearch)
      {
        // case 3: No category, but we have a search string
        return Prods.Where(p => p.Title.ToLower().Contains(searchLow) || p.Description.ToLower().Contains(searchLow)).ToList();
      }
      // case 4: No category or search string
      return Prods.ToArray();
    }
  }
}
