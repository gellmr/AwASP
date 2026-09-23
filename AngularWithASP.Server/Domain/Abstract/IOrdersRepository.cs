using AngularWithASP.Server.Domain.StoredProc;
using AngularWithASP.Server.DTO.MyOrders;

namespace AngularWithASP.Server.Domain.Abstract
{
  public interface IOrdersRepository
  {
    public Task<bool> SaveOrderAsync(Order order);
    Task<IEnumerable<AdminOrderRow>> GetOrdersWithUsersAsync(string? backlogSearch, Int32 pageNum, Int32 pageSize);
    public IEnumerable<Order>? GetUserOrders(string? idval, string? usertype);
    public Order GetOrderById(int orderid);
    public IEnumerable<Order>? GetMyOrders(string? uid, Guid? gid);
    public Task<IEnumerable<Order>?> GetAllOrdersAsync();
    public Task MergeGuestOrdersIntoUserAsync(Guid guestId, string userId);
  }
}

