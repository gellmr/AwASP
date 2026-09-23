using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Domain.StoredProc;
using AngularWithASP.Server.DTO.MyOrders;
using AngularWithASP.Server.Infrastructure;
using System.Globalization;

namespace AngularWithASP.Server.Domain
{
  public class EFOrdersRepository: IOrdersRepository
  {
    private readonly IConfiguration _config;
    private StoreContext context;

    public EFOrdersRepository(IConfiguration c){
      _config = c;
      context = new StoreContext(_config);
    }

    // Return true if saved successfully.
    public async Task<bool> SaveOrderAsync(Order order)
    {
      bool exists = (order.ID != null) && await context.Orders.AnyAsync(o => o.ID == order.ID);
      if (exists)
      {
        // Update
        Order dbOrder = await context.Orders.FirstAsync(o => o.ID == order.ID);
        
        dbOrder.UserID = order.UserID;
        dbOrder.GuestID = order.GuestID;

        if (order.ShipAddressID == null){
          // Create
          dbOrder.ShipAddress = order.ShipAddress; // Allow database to auto assign dbOrder.ShipAddressID
          context.Entry(dbOrder.ShipAddress).State = EntityState.Added;
        }
        else{
          // Update
          dbOrder.ShipAddress = order.ShipAddress;
          context.Entry(dbOrder.ShipAddress).State = EntityState.Modified;
          dbOrder.ShipAddressID = order.ShipAddressID;
        }

        if (order.BillAddressID == null){
          // Create
          dbOrder.BillAddress = order.BillAddress; // Allow database to auto assign dbOrder.BillAddressID
          context.Entry(dbOrder.BillAddress).State = EntityState.Added;
        }
        else{
          // Update
          dbOrder.BillAddress = order.BillAddress;
          context.Entry(dbOrder.BillAddress).State = EntityState.Modified;
          dbOrder.BillAddressID = order.BillAddressID;
        }
        
        dbOrder.OrderPayments = order.OrderPayments;
        dbOrder.OrderedProducts = order.OrderedProducts;

        dbOrder.OrderPlacedDate     = order.OrderPlacedDate;
        dbOrder.PaymentReceivedDate = order.PaymentReceivedDate;
        dbOrder.ReadyToShipDate     = order.ReadyToShipDate;
        dbOrder.ShipDate            = order.ShipDate;
        dbOrder.ReceivedDate        = order.ReceivedDate;

        dbOrder.OrderStatus = order.OrderStatus;

        await context.SaveChangesAsync();
        return true;
      }
      else
      {
        // Create new record
        context.Orders.Add(order);
        foreach (OrderedProduct op in order.OrderedProducts){
          InStockProduct ip = op.InStockProduct;
          context.Entry(ip).State = EntityState.Unchanged; // Dont create the product. It already exists in database.
          context.OrderedProducts.Add(op);
        }
        if (order.Guest != null){
          context.Entry(order.Guest).State = EntityState.Unchanged; // Dont create the guest.
        }
        if (order.AppUser != null){
          context.Entry(order.AppUser).State = EntityState.Unchanged; // Dont create the user.
        }
        await context.SaveChangesAsync();
        return true;
      }
      return false;
    }

    public async Task<IEnumerable<AdminOrderRow>> GetOrdersWithUsersAsync(string? backlogSearch, Int32 pageNum=1, Int32 pageSize=12)
    {
      // Compare database DateTime values to our search string by casting them to this format.
      string dateMatchFormat = MyExtensions.DefaultOrderPlacedDateFormat;

      // Call the stored procedure with the new separate parameters
      IEnumerable<AdminOrderRow> rows = await context.AdminOrderRows
          .FromSqlInterpolated($"EXEC GetAdminOrders {pageNum}, {pageSize}, {backlogSearch}, {dateMatchFormat}")
          .ToListAsync();
      return rows;
    }

    public IEnumerable<Order>? GetUserOrders(string? idval, string? usertype)
    {
      idval = (idval != null) ? idval.ToLower() : null;
      if(usertype == null){
        throw new ArgumentException("usertype is missing. Cannot get orders.");
      }
      if (idval != null){
        if (usertype == "user"){
          IEnumerable<Order> ordersUser = context.Orders.Where(o => o.UserID.ToLower().Equals(idval));
          LoadAssociatedRecordsForOrders(ordersUser);
          return ordersUser;
        }
        else if (usertype == "guest"){
          IEnumerable<Order> ordersGuest = context.Orders.Where(o => o.GuestID.ToString().ToLower().Equals(idval));
          LoadAssociatedRecordsForOrders(ordersGuest);
          return ordersGuest;
        }
      }
      return new List<Order>();
    }

    public Order GetOrderById(int orderid)
    {
      Order? order = context.Orders.FirstOrDefault(ord => ord.ID == orderid);
      if (order == null){
        throw new ArgumentException("Could not load Order with orderid " + orderid);
      }
      LoadAssociatedRecords(order);
      return order;
    }

    public IEnumerable<Order>? GetMyOrders(string? uid, Guid? gid)
    {
      // If empty string, just use null. If we have a value, convert to lower.
      uid = string.IsNullOrEmpty(uid) ? null : uid.ToLower();
      
      if (uid != null && gid != null){
        // Look up orders, for the given guest and user ids.
        IEnumerable<Order> ordersBoth = context.Orders.Where(o => o.UserID.ToLower().Equals(uid) || o.GuestID.Equals(gid));
        LoadAssociatedRecordsForOrders(ordersBoth);
        return ordersBoth;
      }
      else if(uid != null)
      {
        // Look up user orders
        IEnumerable<Order> userOrders = context.Orders.Where(o => o.UserID.ToLower().Equals(uid));
        LoadAssociatedRecordsForOrders(userOrders);
        return userOrders;
      }
      // Look up guest orders
      IEnumerable<Order> guestOrders = context.Orders.Where(o => o.GuestID.Equals(gid));
      LoadAssociatedRecordsForOrders(guestOrders);
      return guestOrders;
    }

    public async Task<IEnumerable<Order>?> GetAllOrdersAsync(){
        return await context.Orders.ToListAsync();
      }

      public async Task MergeGuestOrdersIntoUserAsync(Guid guestId, string userId)
      {
        var guestOrders = await context.Orders.Where(o => o.GuestID == guestId).ToListAsync();
        foreach(var o in guestOrders) {
          o.GuestID = null;
          o.Guest = null;
          o.UserID = userId;
        }
        await context.SaveChangesAsync();
      }

    // ------------------------------------------------------------------------------
    // Utility methods below are not part of repo interface

    protected List<OrderPayment> LoadPayments(Order order)
    {
      return context.OrderPayments.Where(pay => pay.OrderID == order.ID).Select(pay => new OrderPayment{
          ID = pay.ID,
          Order = null,   // Avoid load loop
          OrderID = null, // Avoid load loop
          Amount = pay.Amount,
          Date = pay.Date,
        }).ToList();
    }

    protected List<OrderedProduct> LoadOrderedProducts(Order order)
    {
      return context.OrderedProducts.Where(op => op.OrderID == order.ID).
      Select(op => new OrderedProduct
      {
        // Load everything that we might need for display on the My Orders page...
        ID = op.ID,
        Quantity = op.Quantity,
        Order = null,   // Avoid cycle of loading objects. We already have the Order.
        OrderID = null,
        InStockProduct = op.InStockProduct,
        InStockProductID = op.InStockProductID,
      })
      .ToList();
    }

    protected IEnumerable<Order> LoadAssociatedRecordsForOrders(IEnumerable<Order> rows)
    {
      foreach (Order order in rows){
        LoadAssociatedRecords(order);
      }
      return rows;
    }

    protected void LoadAssociatedRecords(Order order)
    {
      order.OrderedProducts = LoadOrderedProducts(order);
      order.OrderPayments = LoadPayments(order);
      order.BillAddress = context.Addresses.FirstOrDefault(adr => adr.ID == order.BillAddressID);
      order.ShipAddress = context.Addresses.FirstOrDefault(adr => adr.ID == order.ShipAddressID);
    }
  }
}

