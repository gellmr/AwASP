using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AngularWithASP.Server.Domain
{
  public class OrderedProduct
  {
    [Key]
    public Nullable<Int32> ID { get; set; } // primary key. set to null, then database will assign a value
    public Int32 Quantity { get; set; }

    [ForeignKey("OrderID")] // use the value of OrderID as foreign key to the Orders table.
    public virtual UOrder? Order { get; set; }  // navigation property.
    public Nullable<Int32> OrderID { get; set; } // foreign key value to use, for Orders table.

    [ForeignKey("InStockProductID")] // use the value of InStockProductID as foreign key to the InStockProducts table.
    public virtual required InStockProduct InStockProduct { get; set; } // navigation property.
    public Nullable<Int32> InStockProductID { get; set; } // foreign key value to use, for InStockProducts table.
  }
}
