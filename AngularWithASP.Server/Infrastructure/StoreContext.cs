using Google;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.StoredProc;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularWithASP.Server.Infrastructure
{
  public class StoreContext : IdentityDbContext<AppUser>
  {
    private IConfiguration _config;

    // Define Entities we want EF to track and perform CRUD operations
    public DbSet<OrderedProduct> OrderedProducts { get; set; }
    public DbSet<InStockProduct> InStockProducts { get; set; }
    public DbSet<CartLine> CartLines { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<AdminOrderRow> AdminOrderRows { get; set; } // Keyless (Stored Procedure)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configure TPH inheritance
      modelBuilder.Entity<UOrder>()
        .ToTable("Orders") // Map to Orders table
        .HasDiscriminator<string>("OrderType") // Add a discriminator column
        .HasValue<OrderV1>("OrderV1") // The value to use for legacy orders
        .HasValue<Order>("OrderV2");  // The value to use for current orders

      modelBuilder.Entity<UOrder>()
      .HasOne(o => o.AppUser)
      .WithMany(u => u.Orders)
      .HasForeignKey(o => o.UserID)
      .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<UOrder>()
      .HasOne(o => o.Guest)
      .WithMany(u => u.Orders)
      .HasForeignKey(o => o.GuestID)
      .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Order>().HasBaseType<UOrder>();

      modelBuilder.Entity<Order>()
      .HasOne(o => o.ShipAddress);

      modelBuilder.Entity<Order>()
      .HasOne(o => o.BillAddress);

      modelBuilder.Entity<OrderedProduct>()
      .HasOne(p => p.Order)
      .WithMany(o => o.OrderedProducts)
      .HasForeignKey(p => p.OrderID)
      .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<OrderedProduct>()
      .HasOne(o => o.InStockProduct);

      modelBuilder.Entity<OrderPayment>()
      .HasOne(p => p.Order)
      .WithMany(o => o.OrderPayments)
      .HasForeignKey(p => p.OrderID)
      .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<AdminOrderRow>()
      .HasNoKey()
      .ToTable("AdminOrderRows", t => t.ExcludeFromMigrations());
    }

    public StoreContext(IConfiguration c) : base(){
      _config = c;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      // Runs during startup, after connection string is loaded from JSON config file. (Not in source control)
      string conn = _config.GetConnectionString("StoreContext");
      optionsBuilder.UseSqlServer(conn);
    }
  }
}
