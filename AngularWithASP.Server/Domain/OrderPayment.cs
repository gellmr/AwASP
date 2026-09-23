using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AngularWithASP.Server.Domain
{
  public class OrderPayment
  {
    [Key]
    public Nullable<Int32> ID { get; set; } // primary key

    [ForeignKey("OrderID")] // use the value of OrderID as foreign key to the Order table.
    public virtual UOrder? Order { get; set; } // navigation property.
    public Nullable<Int32> OrderID { get; set; } // foreign key value to use, for Order table.

    public Nullable<Decimal> Amount { get; set; }
    public DateTimeOffset Date { get; set; }
  }
}
