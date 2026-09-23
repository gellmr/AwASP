using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Domain
{
  public enum ShippingState
  {
    NotYetPlaced,
    OrderPlaced,
    PaymentReceived,
    ReadyToShip,
    Shipped,
    Received
  }

  public abstract class OrderBase
  {
    [Key]
    public Nullable<Int32> ID { get; set; } // primary key for Order

    // AppUser 1-----* Order
    [ForeignKey("UserID")] // use the value of UserID as foreign key to the AspNetUsers table.
    public virtual AppUser AppUser { get; set; } // navigation property.
    public string? UserID { get; set; } // foreign key value to use, for AspNetUsers table.

    // Guest 1-----* Order
    [ForeignKey("GuestID")] // use the value of GuestID as foreign key to the Guests table.
    public virtual Guest Guest { get; set; } // navigation property.
    public Nullable<Guid> GuestID { get; set; } // foreign key value to use, for Guests table. Null if the user was logged in when they placed order.

    public virtual IList<OrderPayment> OrderPayments { get; set; }
    public virtual IList<OrderedProduct> OrderedProducts { get; set; }

    public Nullable<DateTimeOffset> OrderPlacedDate { get; set; }
    public Nullable<DateTimeOffset> PaymentReceivedDate { get; set; }
    public Nullable<DateTimeOffset> ReadyToShipDate { get; set; }
    public Nullable<DateTimeOffset> ShipDate { get; set; }
    public Nullable<DateTimeOffset> ReceivedDate { get; set; }

    public string OrderStatus { get; set; }

    [NotMapped]
    public string UserOrGuestId
    {
      get { return string.IsNullOrEmpty(UserID) ? GuestID.ToString() : UserID; }
    }

    [NotMapped]
    public string UserOrGuestName
    {
      get {
        if (!string.IsNullOrEmpty(UserID)){
          return AppUser == null ? string.Empty : AppUser.UserName;
        }
        else if (GuestID != null){
          return Guest == null ? string.Empty : Guest.FullName;
        }
        else {
          return string.Empty;
        }
      }
    }

    [NotMapped]
    public string AccountType
    {
      get { return (GuestID != null) ? "Guest" : "User"; }
    }

    [NotMapped]
    public string UserOrGuestEmail
    {
      get {
        if (GuestID != null && Guest != null){
          return Guest.Email;
        }
        if (UserID != null && AppUser != null){
          return AppUser.Email;
        }
        return string.Empty;
      }
    }

    [NotMapped]
    public string ItemString
    {
      get
      {
        try
        {
          if (OrderedProducts == null || OrderedProducts.Count == 0){
            return string.Empty;
          }
          int sentenceMax = 40;
          List<OrderedProduct> orderedProds = OrderedProducts.ToList();
          StringBuilder builder = new StringBuilder();
          if (orderedProds.Count > 0)
          {
            int c = orderedProds.Count;
            for (int i = 0; i < c; i++)
            {
              string productName = orderedProds[i].InStockProduct.Title;
              Int32 qty = orderedProds[i].Quantity;
              string qtyString = " (x" + qty.ToString() + ") ";
              builder.Append(productName + qtyString + ((i < c - 1) ? ", " : "")); // "Life Jacket (x2), "
            }
          }
          string productList = MyExtensions.Truncate(builder.ToString(), sentenceMax);
          return productList; // eg "Kayak (x2), Life Jacket (x2), Polycarbon Injection M..."
        }
        catch(Exception e)
        {
          return string.Empty;
        }
      }
    }

    [NotMapped]
    public Decimal OrderPaymentsReceived
    {
      get
      {
        Decimal sum = 0;
        if (OrderPayments == null || OrderPayments.Count == 0)
        {
          return 0;
        }
        foreach (OrderPayment payment in OrderPayments)
        {
          sum += (payment.Amount ?? 0);
        }
        return sum;
      }
    }

    [NotMapped]
    public Decimal Outstanding
    {
      get { return PriceTotal - OrderPaymentsReceived; }
    }

    [NotMapped]
    public Decimal QuantityTotal
    {
      get
      {
        Decimal sum = 0;
        foreach (OrderedProduct op in OrderedProducts)
        {
          sum += op.Quantity;
        }
        return sum;
      }
    }

    [NotMapped]
    public Decimal PriceTotal
    {
      get
      {
        Decimal sum = 0;
        foreach (OrderedProduct op in OrderedProducts)
        {
          sum += (op.InStockProduct.Price * op.Quantity);
        }
        return sum;
      }
    }

    public static string ParseAddress(CheckoutSubmitDTO shippingInfo)
    {
      string? ne = string.Empty;
      string line1 = string.IsNullOrEmpty(shippingInfo.ShipLine1) ? ne : (shippingInfo.ShipLine1 + ", ");
      string line2 = string.IsNullOrEmpty(shippingInfo.ShipLine2) ? ne : (shippingInfo.ShipLine2 + ", ");
      string line3 = string.IsNullOrEmpty(shippingInfo.ShipLine3) ? ne : (shippingInfo.ShipLine3 + ", ");
      return line1 + line2 + line3 +
        shippingInfo.ShipCity + ", " +
        shippingInfo.ShipState + ", " +
        shippingInfo.ShipCountry + ", " +
        shippingInfo.ShipZip;
    }

    public static ShippingState ParseShippingState(string str)
    {
      ShippingState myState;
      ShippingState state = ShippingState.NotYetPlaced;
      try
      {
        if (Enum.TryParse(str, out myState))
        {
          switch (myState)
          {
            case ShippingState.NotYetPlaced: state = ShippingState.NotYetPlaced; break;
            case ShippingState.OrderPlaced: state = ShippingState.OrderPlaced; break;
            case ShippingState.PaymentReceived: state = ShippingState.PaymentReceived; break;
            case ShippingState.ReadyToShip: state = ShippingState.ReadyToShip; break;
            case ShippingState.Shipped: state = ShippingState.Shipped; break;
            case ShippingState.Received: state = ShippingState.Received; break;
          }
          return state;
        }
        throw new Exception("could not parse string as ShippingState enum");
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public static string ParseShippingState(ShippingState myState)
    {
      string myString = "";
      switch (myState)
      {
        case ShippingState.NotYetPlaced: myString = "NotYetPlaced"; break;
        case ShippingState.OrderPlaced: myString = "OrderPlaced"; break;
        case ShippingState.PaymentReceived: myString = "PaymentReceived"; break;
        case ShippingState.ReadyToShip: myString = "ReadyToShip"; break;
        case ShippingState.Shipped: myString = "Shipped"; break;
        case ShippingState.Received: myString = "Received"; break;
      }
      return myString;
    }

    // Get an object to convert to JSON, for responding to ajax requests.
    public OrderDetailUpdateDTO GetUpdateDTO()
    {
      OrderDetailUpdateDTO response = new OrderDetailUpdateDTO();
      response.QuantityTotal = QuantityTotal;
      response.PriceTotal = PriceTotal;
      response.Rows = new List<OrderDetailUpdateRowDTO>();
      foreach (OrderedProduct p in OrderedProducts)
      {
        OrderDetailUpdateRowDTO row = new OrderDetailUpdateRowDTO();
        row.ProductID = p.InStockProduct.ID;
        row.ProductName = p.InStockProduct.Title;
        row.UnitPrice = p.InStockProduct.Price;
        row.Quantity = p.Quantity;
        row.Cost = p.InStockProduct.Price * p.Quantity;
        row.Category = ProductCategory.ParseCat(p.InStockProduct.Category);
        response.Rows.Add(row);
      }
      return response;
    }

    // Enum for sorting orders, on the backlog page.
    public enum OrderSortEnum
    {
      None,
      OrderID,
      Username,
      UserID,
      AccountType,
      Email,
      OrderPlaced,
      PaymentReceived,
      Outstanding,
      ItemsOrdered,
      Items,
      OrderStatus,
      Edit
    }

    // Convert a given string into OrderSortEnum
    public static OrderSortEnum ParseOrderSortEnum(string str)
    {
      OrderSortEnum outEnum;
      OrderSortEnum myEnum = OrderSortEnum.OrderID;
      try
      {
        if (String.IsNullOrEmpty(str))
        {
          return OrderSortEnum.None;
        }
        if (Enum.TryParse(str, out outEnum))
        {
          switch (outEnum)
          {
            case OrderSortEnum.OrderID: myEnum = OrderSortEnum.OrderID; break;
            case OrderSortEnum.Username: myEnum = OrderSortEnum.Username; break;
            case OrderSortEnum.UserID: myEnum = OrderSortEnum.UserID; break;
            case OrderSortEnum.AccountType: myEnum = OrderSortEnum.AccountType; break;
            case OrderSortEnum.Email: myEnum = OrderSortEnum.Email; break;
            case OrderSortEnum.OrderPlaced: myEnum = OrderSortEnum.OrderPlaced; break;
            case OrderSortEnum.PaymentReceived: myEnum = OrderSortEnum.PaymentReceived; break;
            case OrderSortEnum.Outstanding: myEnum = OrderSortEnum.Outstanding; break;
            case OrderSortEnum.ItemsOrdered: myEnum = OrderSortEnum.ItemsOrdered; break;
            case OrderSortEnum.Items: myEnum = OrderSortEnum.Items; break;
            case OrderSortEnum.OrderStatus: myEnum = OrderSortEnum.OrderStatus; break;
          }
          return myEnum;
        }
        throw new Exception("could not parse string as OrderSortEnum enum");
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }

  // This class is converted into JSON for responding to the Detail page update requests.
  public class OrderDetailUpdateRowDTO
  {
    public Int32 ProductID { get; set; }
    public string ProductName { get; set; }
    public Decimal UnitPrice { get; set; }
    public Int32 Quantity { get; set; }
    public Decimal Cost { get; set; }
    public string Category { get; set; }
  }

  // This class is converted into JSON for responding to the Detail page update requests.
  public class OrderDetailUpdateDTO
  {
    public List<OrderDetailUpdateRowDTO> Rows { get; set; }
    public Decimal QuantityTotal { get; set; }
    public Decimal PriceTotal { get; set; }
  }
}
