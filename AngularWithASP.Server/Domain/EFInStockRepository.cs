using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Domain
{
  public class EFInStockRepository : IInStockRepository
  {
    private readonly IConfiguration _config;
    private StoreContext context;

    public EFInStockRepository(IConfiguration c)
    {
      _config = c;
      context = new StoreContext(_config);
    }

    public IEnumerable<InStockProduct> InStockProducts
    {
      get { return context.InStockProducts; }
    }
  }
}
