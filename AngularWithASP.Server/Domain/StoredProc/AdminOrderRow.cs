using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AngularWithASP.Server.DTO;

namespace AngularWithASP.Server.Domain.StoredProc
{
  // This class is used to map to the results of the CreateSPGetAdminOrders stored procedure.
  [Keyless]
  public class AdminOrderRow
  {
    private static Int32 maxLenItemsDisplay = 30;

    public Int32 OrderID { get; set; }
    public Int32 RowNumber { get; set; } // The row number eg 1,2,3 of the current row, from the total result set. Continues to next page.
    public string? Username { get; set; }
    public string? UserID { get; set; }
    public string? GuestID { get; set; }
    public string AccountType { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset OrderPlaced { get; set; }
    public decimal? PaymentReceived { get; set; }
    public decimal? Outstanding { get; set; }
    public Int32? ItemsOrdered { get; set; }
    public string? Items { get; set; }
    public string OrderStatus { get; set; }
    public Int32 TotalRows { get; set; } // Eg The total number of rows found by the query (but we only return one page from this set)

    public OrderSlugDTO OrderSlug {
      get{
        string itemDisplay = Items.IsNullOrEmpty() ? string.Empty : ((Items.Length > maxLenItemsDisplay) ? Items.Substring(0, maxLenItemsDisplay - 3) + "..." : Items);
        string UserIDshort  = (UserID == null)  ? null : ((UserID.Length  < 12) ? UserID  : (UserID.Substring(0, 8)  + "..."));
        string GuestIDshort = (GuestID == null) ? null : ((GuestID.Length < 12) ? GuestID : (GuestID.Substring(0, 8) + "..."));
        return new OrderSlugDTO{
          ID = OrderID.ToString(),
          RowNumber = RowNumber,
          Username = Username ?? string.Empty,
          UserID = UserID,
          GuestID = GuestID,
          UserIDshort = UserIDshort,
          GuestIDshort = GuestIDshort,
          AccountType = AccountType,
          Email = Email ?? string.Empty,
          OrderPlacedDate = OrderPlaced.ToString(Infrastructure.MyExtensions.DefaultOrderPlacedDateFormat),
          PaymentReceivedAmount = PaymentReceived.ToString() ?? string.Empty,
          Outstanding = Outstanding.ToString() ?? string.Empty,
          ItemsOrdered = ItemsOrdered.ToString() ?? string.Empty,
          Items = itemDisplay,
          OrderStatus = OrderStatus,
          TotalRows = TotalRows
        };
      }
    }
  }
}
