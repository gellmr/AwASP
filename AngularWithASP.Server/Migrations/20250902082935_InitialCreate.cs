using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularWithASP.Server.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "AspNetRoles",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetRoles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Guests",
          columns: table => new
          {
            ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Picture = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Guests", x => x.ID);
          });

      migrationBuilder.CreateTable(
          name: "InStockProducts",
          columns: table => new
          {
            ID = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Category = table.Column<int>(type: "int", nullable: false),
            Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_InStockProducts", x => x.ID);
          });

      migrationBuilder.CreateTable(
          name: "AspNetRoleClaims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUsers",
          columns: table => new
          {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            GuestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            table.ForeignKey(
                      name: "FK_AspNetUsers_Guests_GuestID",
                      column: x => x.GuestID,
                      principalTable: "Guests",
                      principalColumn: "ID");
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserClaims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserLogins",
          columns: table => new
          {
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            table.ForeignKey(
                      name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserRoles",
          columns: table => new
          {
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
            table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserTokens",
          columns: table => new
          {
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey(
                      name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "CartLines",
          columns: table => new
          {
            ID = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Quantity = table.Column<int>(type: "int", nullable: false),
            UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
            GuestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            InStockProductID = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CartLines", x => x.ID);
            table.ForeignKey(
                      name: "FK_CartLines_AspNetUsers_UserID",
                      column: x => x.UserID,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_CartLines_Guests_GuestID",
                      column: x => x.GuestID,
                      principalTable: "Guests",
                      principalColumn: "ID");
            table.ForeignKey(
                      name: "FK_CartLines_InStockProducts_InStockProductID",
                      column: x => x.InStockProductID,
                      principalTable: "InStockProducts",
                      principalColumn: "ID",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Orders",
          columns: table => new
          {
            ID = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
            GuestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            OrderPlacedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            PaymentReceivedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            ReadyToShipDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            ShipDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            ReceivedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            BillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
            OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Orders", x => x.ID);
            table.ForeignKey(
                      name: "FK_Orders_AspNetUsers_UserID",
                      column: x => x.UserID,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Orders_Guests_GuestID",
                      column: x => x.GuestID,
                      principalTable: "Guests",
                      principalColumn: "ID",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "OrderedProducts",
          columns: table => new
          {
            ID = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Quantity = table.Column<int>(type: "int", nullable: false),
            OrderID = table.Column<int>(type: "int", nullable: true),
            InStockProductID = table.Column<int>(type: "int", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OrderedProducts", x => x.ID);
            table.ForeignKey(
                      name: "FK_OrderedProducts_InStockProducts_InStockProductID",
                      column: x => x.InStockProductID,
                      principalTable: "InStockProducts",
                      principalColumn: "ID");
            table.ForeignKey(
                      name: "FK_OrderedProducts_Orders_OrderID",
                      column: x => x.OrderID,
                      principalTable: "Orders",
                      principalColumn: "ID",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "OrderPayments",
          columns: table => new
          {
            ID = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            OrderID = table.Column<int>(type: "int", nullable: true),
            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OrderPayments", x => x.ID);
            table.ForeignKey(
                      name: "FK_OrderPayments_Orders_OrderID",
                      column: x => x.OrderID,
                      principalTable: "Orders",
                      principalColumn: "ID",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateIndex(
          name: "IX_AspNetRoleClaims_RoleId",
          table: "AspNetRoleClaims",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "RoleNameIndex",
          table: "AspNetRoles",
          column: "NormalizedName",
          unique: true,
          filter: "[NormalizedName] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserClaims_UserId",
          table: "AspNetUserClaims",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserLogins_UserId",
          table: "AspNetUserLogins",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserRoles_RoleId",
          table: "AspNetUserRoles",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "EmailIndex",
          table: "AspNetUsers",
          column: "NormalizedEmail");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUsers_GuestID",
          table: "AspNetUsers",
          column: "GuestID");

      migrationBuilder.CreateIndex(
          name: "UserNameIndex",
          table: "AspNetUsers",
          column: "NormalizedUserName",
          unique: true,
          filter: "[NormalizedUserName] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_CartLines_GuestID",
          table: "CartLines",
          column: "GuestID");

      migrationBuilder.CreateIndex(
          name: "IX_CartLines_InStockProductID",
          table: "CartLines",
          column: "InStockProductID");

      migrationBuilder.CreateIndex(
          name: "IX_CartLines_UserID",
          table: "CartLines",
          column: "UserID");

      migrationBuilder.CreateIndex(
          name: "IX_OrderedProducts_InStockProductID",
          table: "OrderedProducts",
          column: "InStockProductID");

      migrationBuilder.CreateIndex(
          name: "IX_OrderedProducts_OrderID",
          table: "OrderedProducts",
          column: "OrderID");

      migrationBuilder.CreateIndex(
          name: "IX_OrderPayments_OrderID",
          table: "OrderPayments",
          column: "OrderID");

      migrationBuilder.CreateIndex(
          name: "IX_Orders_GuestID",
          table: "Orders",
          column: "GuestID");

      migrationBuilder.CreateIndex(
          name: "IX_Orders_UserID",
          table: "Orders",
          column: "UserID");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "AspNetRoleClaims");

      migrationBuilder.DropTable(
          name: "AspNetUserClaims");

      migrationBuilder.DropTable(
          name: "AspNetUserLogins");

      migrationBuilder.DropTable(
          name: "AspNetUserRoles");

      migrationBuilder.DropTable(
          name: "AspNetUserTokens");

      migrationBuilder.DropTable(
          name: "CartLines");

      migrationBuilder.DropTable(
          name: "OrderedProducts");

      migrationBuilder.DropTable(
          name: "OrderPayments");

      migrationBuilder.DropTable(
          name: "AspNetRoles");

      migrationBuilder.DropTable(
          name: "InStockProducts");

      migrationBuilder.DropTable(
          name: "Orders");

      migrationBuilder.DropTable(
          name: "AspNetUsers");

      migrationBuilder.DropTable(
          name: "Guests");
    }
  }
}
