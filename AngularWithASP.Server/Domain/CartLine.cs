using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularWithASP.Server.Domain
{
  public class CartLine
  {
    [Key]
    public Nullable<Int32> ID { get; set; } // Primary key. set to null, then database will assign a value
    public Int32 Quantity { get; set; } // How many of this item are currently in the customer's cart

    // AppUser 1-----* Order
    [ForeignKey("UserID")] // Use the value of UserID as foreign key to the AspNetUsers table.
    public virtual AppUser? AppUser { get; set; } // Navigation property.
    public string? UserID { get; set; } // Foreign key value to use, for AspNetUsers table.

    // Guest 1-----* Order
    [ForeignKey("GuestID")] // Use the value of GuestID as foreign key to the Guests table.
    public virtual Guest? Guest { get; set; } // Navigation property.
    public Nullable<Guid> GuestID { get; set; } // Foreign key value to use, for Guests table. Null if the user was logged in when they placed order.

    [ForeignKey("InStockProductID")] // Use the value of InStockProductID as foreign key to the InStockProducts table.
    public virtual InStockProduct InStockProduct { get; set; } // Navigation property.
    public Int32 InStockProductID { get; set; } // Foreign key value to use, for InStockProducts table.

    [NotMapped]
    public Decimal PriceTotal {
      get {
        return InStockProduct.Price * Quantity;
      }
    }
  }
}
