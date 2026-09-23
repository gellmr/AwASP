namespace AngularWithASP.Server.Domain
{
  public class UOrder : OrderBase
  {
    public UOrder()
    {
      GuestID = null;
      UserID = null;
      ID = null;
      OrderedProducts = new List<OrderedProduct>();
    }

    public UOrder(int orderID, string userID, Nullable<Guid> guestID)
    {
      GuestID = guestID;
      UserID = userID;
      ID = orderID;
      OrderedProducts = new List<OrderedProduct>();
    }

    public UOrder(UOrder ord)
    {
      ID = ord.ID;

      AppUser = ord.AppUser;
      UserID = ord.UserID;

      Guest = ord.Guest;
      GuestID = ord.GuestID;

      OrderPayments = ord.OrderPayments;
      OrderedProducts = ord.OrderedProducts;

      OrderPlacedDate     = ord.OrderPlacedDate;
      PaymentReceivedDate = ord.PaymentReceivedDate;
      ReadyToShipDate     = ord.ReadyToShipDate;
      ShipDate            = ord.ShipDate;
      ReceivedDate        = ord.ReceivedDate;

      OrderStatus = ord.OrderStatus;
    }
  }
}
