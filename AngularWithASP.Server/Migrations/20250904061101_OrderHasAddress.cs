using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace AngularWithASP.Server.Migrations
{
  public partial class OrderHasAddress : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      // Step 1: Add new columns, named "ShipAddressID" and "BillAddressID" to Orders table. Column type is nullable int.
      migrationBuilder.AddColumn<int>(name: "BillAddressID", table: "Orders", type: "int", nullable: true);
      migrationBuilder.AddColumn<int>(name: "ShipAddressID", table: "Orders", type: "int", nullable: true);

      // Step 2: Create a new table named "Addresses"
      migrationBuilder.CreateTable(
        name: "Addresses",
        columns: table => new
        {
          ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "111, 1"),
          Line1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
          Line2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
          Line3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
          City = table.Column<string>(type: "nvarchar(max)", nullable: false),
          State = table.Column<string>(type: "nvarchar(max)", nullable: false),
          Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
          Zip = table.Column<string>(type: "nvarchar(max)", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Addresses", x => x.ID); // Use ID column as primary key for this table.
        }
      );

      // Step 3: Create an index, on the ShipAddressID and BillAddressID columns of Orders table.
      migrationBuilder.CreateIndex(name: "IX_Orders_BillAddressID", table: "Orders", column: "BillAddressID");
      migrationBuilder.CreateIndex(name: "IX_Orders_ShipAddressID", table: "Orders", column: "ShipAddressID");

      // Step 4: Create foreign key constraints.
      migrationBuilder.AddForeignKey(
          name: "FK_Orders_Addresses_BillAddressID", // Orders.BillAddressID *---1 Addresses.ID
          table: "Orders",                // Dependent table. This will have the foreign key column.
          column: "BillAddressID",        // This is the name of the foreign key column we are creating in Orders table.
          principalTable: "Addresses",    // Principal table, which the foreign key column refers to.
          principalColumn: "ID");         // Primary key which the foreign key column refers to.
      migrationBuilder.AddForeignKey(
          name: "FK_Orders_Addresses_ShipAddressID", // Orders.ShipAddressID *---1 Addresses.ID
          table: "Orders",                // Dependent table. This will have the foreign key column.
          column: "ShipAddressID",        // This is the name of the foreign key column we are creating in Orders table.
          principalTable: "Addresses",    // Principal table, which the foreign key column refers to.
          principalColumn: "ID");         // Primary key which the foreign key column refers to.

      

      // Step 6: Drop the old string columns from Orders table.
      migrationBuilder.DropColumn(name: "BillingAddress", table: "Orders");
      migrationBuilder.DropColumn(name: "ShippingAddress", table: "Orders");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      // Step 6: Restore the old string columns to Orders table.
      migrationBuilder.AddColumn<string>(name: "BillingAddress", table: "Orders", type: "nvarchar(max)", nullable: true);
      migrationBuilder.AddColumn<string>(name: "ShippingAddress", table: "Orders", type: "nvarchar(max)", nullable: true);

      

      // Step 4: Delete the foreign key constraint that we created in Step 4.
      migrationBuilder.DropForeignKey(name: "FK_Orders_Addresses_BillAddressID", table: "Orders");
      migrationBuilder.DropForeignKey(name: "FK_Orders_Addresses_ShipAddressID", table: "Orders");

      // Step 2: Drop the table "Addresses" that we created in Step 2.
      migrationBuilder.DropTable(name: "Addresses");

      // Step 3: Drop the index on the XxxxAddressID column of Orders table that we created in Step 3
      migrationBuilder.DropIndex(name: "IX_Orders_BillAddressID", table: "Orders");
      migrationBuilder.DropIndex(name: "IX_Orders_ShipAddressID", table: "Orders");

      // Step 1: Remove the XxxxAddressID column that we added in Step 1
      migrationBuilder.DropColumn(name: "BillAddressID", table: "Orders");
      migrationBuilder.DropColumn(name: "ShipAddressID", table: "Orders");

    }
  }
}

