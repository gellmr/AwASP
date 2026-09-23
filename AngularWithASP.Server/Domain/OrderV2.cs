using System.ComponentModel.DataAnnotations.Schema;

namespace AngularWithASP.Server.Domain
{
  public class Order : UOrder
  {
    public Order() : base() { }

    // ShipAddress 1-----1 Order
    [ForeignKey("ShipAddressID")] // Use the value of our ShipAddressID as foreign key to the Address table.
    public virtual Address? ShipAddress { get; set; } // Navigation property.
    public Int32? ShipAddressID { get; set; } // The PK from Address table

    // BillAddress 1-----1 Order
    [ForeignKey("BillAddressID")] // Use the value of our BillAddressID as foreign key to the Address table.
    public virtual Address? BillAddress { get; set; } // Navigation property.
    public Int32? BillAddressID { get; set; } // The PK from Address table

    public Order(UOrder ord) : base(ord) {}
  }
}
