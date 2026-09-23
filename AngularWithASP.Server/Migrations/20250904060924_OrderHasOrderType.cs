using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularWithASP.Server.Migrations
{
  public partial class OrderHasOrderType : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder){
      migrationBuilder.AddColumn<string>(name: "OrderType", table: "Orders", nullable: true, unicode: true, maxLength: 256);
    }

    protected override void Down(MigrationBuilder migrationBuilder){
      migrationBuilder.DropColumn(name: "OrderType", table: "Orders");
    }
  }
}
