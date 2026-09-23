using Microsoft.AspNet.Identity; // Provides PasswordHasher.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AngularWithASP.Server.Domain;
using System.Diagnostics;

namespace AngularWithASP.Server.Infrastructure
{
  public class InStockProductSeederDTO // Temporary class to help us populate our objects from JSON
  {
    public int? ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public string? Image {get; set; }
  }

  public class AppUserSeederDTO
  {
    public bool IsGuest { get; set; }

    public Guid? Id { get; set; }
    public Guid? GuestID { get; set; }
    public string? Email {get; set;}
    public bool EmailConfirmed { get; set; }
    public string? SecurityStamp { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed {get; set; }
    public bool TwoFactorEnabled { get; set; }
    public Double? LockoutEndDateUtc { get; set; }
    public bool LockoutEnabled { get; set; }
    public Int32 AccessFailedCount { get; set; }
    public string? UserName { get; set; }
    public string? Picture { get; set; }
  }

  public class OrderSeederDTO
  {
    public Guid? UserID { get; set; }
    public Guid? GuestID { get; set; }
    public Int32? ID { get; set; }
    public string OrderPlacedDate { get; set; }
    public string PaymentReceivedDate { get; set; }
    public string ReadyToShipDate { get; set; }
    public string ShipDate { get; set; }
    public string ReceivedDate { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? OrderStatus { get; set; }
  }

  public class OrderedProductSeederDTO
  {
    public Int32? ID { get; set; }
    public Int32 Quantity { get; set; }
    public Int32? OrderID { get; set; }
    public Int32? InStockProductID { get; set; }
  }

  public class OrderPaymentSeederDTO
  {
    public Int32? ID { get; set; }
    public Int32? OrderID { get; set; }
    public Decimal? Amount { get; set; }
    public DateTimeOffset? Date { get; set; }
  }

  public class DataSeeder
  {
    private IConfiguration _config;
    private StoreContext _context;

    private Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;
    private Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;

    protected RandomUserMeApiClient _userMeService;

    public static ILookupNormalizer _normalizer;
    public static IPasswordHasher<AppUser> _hasher;

    private static string? _hashedVipPassword;

    public static List<AppUserSeederDTO> appUserDTOs;
    public static IList<AppUser> AppUsers;

    public static IDictionary<string, Guest> Guests;

    public static List<InStockProductSeederDTO> inStockDTOs;
    public static IList<InStockProduct> InStockProducts;

    public static List<OrderSeederDTO> orderDTOs;
    public static IList<Order> Orders;

    public static List<OrderedProductSeederDTO> orderedProductDTOs;
    public static IList<OrderedProduct> OrderedProducts;

    public static List<OrderPaymentSeederDTO> orderPaymentDTOs;
    public static IList<OrderPayment> OrderPayments;

    public DataSeeder(
      StoreContext ctx,
      IConfiguration config,
      Microsoft.AspNetCore.Identity.UserManager<AppUser> um,
      Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> rm,
      ILookupNormalizer norm,
      RandomUserMeApiClient userMeService
    )
    {
      _context = ctx;
      _config = config;
      _userManager = um;
      _roleManager = rm;
      _normalizer = norm;
      _userMeService = userMeService;
    }

    public async Task Execute()
    {
      Console.WriteLine("Begin transaction for data seeding...");
      if (_context.Orders.Any()) {
        Console.WriteLine("Database is already seeded. Skipping seeder.");
        return;
      }
      await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        await Seed();
        await transaction.CommitAsync();
        Console.WriteLine("Data seeded successfully within a transaction.");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        Console.WriteLine($"Error seeding data: {ex.Message}");
        throw;
      }
    }

    private async Task Seed()
    {
      // We want to keep CartLine records, and Guest records. All other tables can be cleared.

      // Delete all rows for the tables we are recreating...
      try{
        _context.OrderPayments.RemoveRange(_context.OrderPayments);
        _context.OrderedProducts.RemoveRange(_context.OrderedProducts);
        _context.InStockProducts.RemoveRange(_context.InStockProducts);
        _context.Users.RemoveRange(_context.Users);
        _context.CartLines.RemoveRange(_context.CartLines);
        _context.Guests.RemoveRange(_context.Guests);
        _context.SaveChanges();
      }catch(Exception ex)
      {
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("Could not clear database tables");
      }

      Guests = new Dictionary<string, Guest>();

      // Add the VIP AppUser
      string vipUserName = _config.GetSection("Authentication:VIP:UserName").Value;
      string vipPassword = _config.GetSection("Authentication:VIP:Password").Value;
      IPasswordHasher hasher = new PasswordHasher();
      _hashedVipPassword = hasher.HashPassword(vipPassword);

      // Seed Roles
      string[] roleNames = { "Admin" };
      //string[] roleNames = { "Admin", "User" }; // Other user types can be added here, eg "Customer"
      foreach (var roleName in roleNames)
      {
        if (!await _roleManager.RoleExistsAsync(roleName)){
          await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
      }

      // Seed User
      AppUser vipAppUser = new AppUser{
        Id = _config.GetSection("Authentication:VIP:Id").Value,
        UserName = vipUserName,
        FullName = "Administrator",
        PasswordHash = _hashedVipPassword,
        //IsGuest = _config.GetSection("Authentication:VIP:IsGuest").Value,
        Email = _config.GetSection("Authentication:VIP:Email").Value,
        EmailConfirmed = Boolean.Parse(_config.GetSection("Authentication:VIP:EmailConfirmed").Value),
        //SecurityStamp = _config.GetSection("Authentication:VIP:SecurityStamp").Value, // Allow database to set this value.
        PhoneNumber = _config.GetSection("Authentication:VIP:PhoneNumber").Value,
        PhoneNumberConfirmed = Boolean.Parse(_config.GetSection("Authentication:VIP:PhoneNumberConfirmed").Value),
        TwoFactorEnabled = Boolean.Parse(_config.GetSection("Authentication:VIP:TwoFactorEnabled").Value),
        //LockoutEndDateUtc = _config.GetSection("Authentication:VIP:LockoutEndDateUtc").Value,
        LockoutEnabled = Boolean.Parse(_config.GetSection("Authentication:VIP:LockoutEnabled").Value),
        AccessFailedCount = Int32.Parse(_config.GetSection("Authentication:VIP:AccessFailedCount").Value),
      };

      try
      {
        // Populate Users
        // [dbo].[AspNetUsers] does not need us to set IDENTITY_INSERT on, as it already allows PK insertion.
        // [dbo].[Guests]      does not need us to set IDENTITY_INSERT on, as it already allows PK insertion.
        AppUsers = new List<AppUser> { vipAppUser }; _context.Users.Add(vipAppUser); _context.SaveChanges();
        appUserDTOs = _config.GetSection("users").Get<List<AppUserSeederDTO>>();
        for (int u = 0; u < (appUserDTOs?.Count ?? 0); u++) { SeedAppUsers(u); }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw;
      }

      // Populate Orders
      try
      {
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Orders] ON;");
        Orders = new List<Order>();
        orderDTOs = _config.GetSection("orders").Get<List<OrderSeederDTO>>();
        for (int oidx = 0; oidx < (orderDTOs?.Count ?? 0); oidx++) { SeedOrders(oidx); }
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Orders] OFF;");
      }
      catch(Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw;
      }

      // Populate InStockProducts
      try
      {
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[InStockProducts] ON;");
        InStockProducts = new List<InStockProduct>();
        inStockDTOs = _config.GetSection("instockproducts").Get<List<InStockProductSeederDTO>>();
        for (int pIdx = 0; pIdx < (inStockDTOs?.Count ?? 0); pIdx++) { SeedInStockProducts(pIdx); }
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[InStockProducts] OFF;");
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw;
      }

      // Populate OrderedProducts
      try
      {
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[OrderedProducts] ON;");
        OrderedProducts = new List<OrderedProduct>();
        orderedProductDTOs = _config.GetSection("orderedproducts").Get<List<OrderedProductSeederDTO>>();
        for (int idx = 0; idx < (orderedProductDTOs?.Count ?? 0); idx++) { SeedOrderedProduct(idx); }
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[OrderedProducts] OFF;");
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw;
      }

      // Populate OrderPayments
      try
      {
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[OrderPayments] ON;");
        OrderPayments = new List<OrderPayment>();
        orderPaymentDTOs = _config.GetSection("orderpayments").Get<List<OrderPaymentSeederDTO>>();
        for (int idx = 0; idx < (orderPaymentDTOs?.Count ?? 0); idx++) { SeedOrderPayment(idx); }
        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[OrderPayments] OFF;");
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        throw;
      }

      // Finished adding records.
    }

    private static DateTimeOffset? GetLockoutUtcDaysFromNow(Double? days)
    {
      return (days == null) ? (DateTimeOffset?)null : (DateTimeOffset.UtcNow.AddDays(Double.Parse(days.ToString() ?? string.Empty)));
    }

    private string? SpliceUsermeEmail(string thisName, string intoThisAddy)
    {
      string? name = thisName.Split('@')[0];
      string? email = intoThisAddy.Split('@')[1];
      return name + "@" + email;
    }

    private void SeedAppUsers(int u)
    {
      AppUserSeederDTO? dto = appUserDTOs?[u];
      if (dto == null) return;

      // Handle potential null UserName gracefully
      string userName = dto.UserName ?? "Default User";
      string[] splitName = userName.Split(" ");
      
      string? appUserId = (dto.Id == null) ? string.Empty : dto.Id.ToString().ToLower();
      if (dto.IsGuest == false)
      {
        AppUser user = new AppUser
        {
          Id = appUserId,
          GuestID = null, // dto.GuestID,
          Guest = null,   // guest,
          Email = dto.Email,
          EmailConfirmed = dto.EmailConfirmed,
          PasswordHash = _hashedVipPassword,
          PhoneNumber = dto.PhoneNumber,
          PhoneNumberConfirmed = dto.PhoneNumberConfirmed,
          TwoFactorEnabled = dto.TwoFactorEnabled,
          LockoutEnd = GetLockoutUtcDaysFromNow(dto.LockoutEndDateUtc),
          LockoutEnabled = true, // "opt in" to lockout functionality. This does not mean the user is locked out.
          AccessFailedCount = dto.AccessFailedCount,
          UserName = MyExtensions.GenUserName(dto.UserName, dto.Id.ToString()),
          FullName = dto.UserName, // "Eg "Diana Walters"
          Picture = (dto.Picture == null) ? string.Empty : dto.Picture
        };
        AppUsers.Add(user);
        _context.Users.Add(user);
      }
      else
      {

        Guest? guest = new Guest{
          ID = (Guid)dto.GuestID,
          Email = dto.Email,
          Picture = (dto.Picture == null) ? string.Empty : dto.Picture,
          FirstName = splitName.Length > 0 ? splitName[0] : "Guest",
          LastName = splitName.Length > 1 ? splitName[1] : "User"
        };

        Guests.Add(appUserId, guest); // record which AppUser this Guest belongs to
        _context.Guests.Add(guest);
      }
      _context.SaveChanges();
    }

    private void SeedInStockProducts(int idx){
      InStockProductSeederDTO dto = inStockDTOs[idx];
      InStockProduct prod = new InStockProduct{
        ID = (Int32)dto.ID,
        Title = dto.Name,
        Description = dto.Description,
        Price = (decimal)dto.Price,
        Category = ProductCategory.ParseCat(dto.Category),
        Image = dto.Image
      };
      InStockProducts.Add(prod);
      _context.InStockProducts.Add(prod);
      _context.SaveChanges();
    }

    private static Nullable<DateTimeOffset> GetOrderDateTime(string input){
      try{
        return (Nullable<DateTimeOffset>)DateTimeOffset.Parse(input);
      }
      catch (FormatException e){
        return null;
      }
    }

    private void SeedOrders(int oidx)
    {
      OrderSeederDTO dto = orderDTOs[oidx];
      Guid? userId = dto.UserID;
      Guid? guestId = dto.GuestID;
      AppUser? user = AppUsers.FirstOrDefault(u => (Guid.Parse(u.Id) == userId) );
      Order order = new Order
      {
        ID = dto.ID,
        OrderPlacedDate = GetOrderDateTime(dto.OrderPlacedDate),
        PaymentReceivedDate = GetOrderDateTime(dto.PaymentReceivedDate),
        ReadyToShipDate = GetOrderDateTime(dto.ReadyToShipDate),
        ShipDate = GetOrderDateTime(dto.ShipDate),
        ReceivedDate = GetOrderDateTime(dto.ReceivedDate),
        OrderStatus = dto.OrderStatus ?? string.Empty,
      };
      
      var parser = new AddressParser(null, null);
      var billDto = parser.ParseAddress(dto.BillingAddress ?? string.Empty);
      var shipDto = parser.ParseAddress(dto.ShippingAddress ?? string.Empty);
      
      order.BillAddress = MyAddressDto.ToAddress(billDto);
      order.ShipAddress = MyAddressDto.ToAddress(shipDto);
      if (user == null)
      {
        // Look up the Guest record for this appUserId
        Guest guest = Guests.First(g => g.Value.ID == guestId).Value;
        // Order belongs to a guest
        order.Guest = guest;
        order.GuestID = guest.ID;
      }
      else
      {
        // Order belongs to an AppUser
        order.AppUser = user;
        order.UserID = userId.ToString();
      }
      // Save the order
      Orders.Add(order);
      _context.Orders.Add(order);
      _context.SaveChanges();
    }

    private void SeedOrderedProduct(int idx)
    {
      OrderedProductSeederDTO dto = orderedProductDTOs[idx];

      InStockProduct isp = InStockProducts.FirstOrDefault(p => p.ID == dto.InStockProductID); // lookup navigation object
      Order order = Orders.FirstOrDefault(o => o.ID == dto.OrderID);                        // lookup navigation object
      OrderedProduct op = new OrderedProduct
      {
        ID = dto.ID,
        Order = order,        // OrderID = dto.OrderID,
        InStockProduct = isp, // InStockProductID = dto.InStockProductID,
        Quantity = dto.Quantity
      };
      OrderedProducts.Add(op);
      _context.OrderedProducts.Add(op);
      _context.SaveChanges();
    }

    private void SeedOrderPayment(int idx)
    {
      OrderPaymentSeederDTO dto = orderPaymentDTOs[idx];

      Order order = Orders.FirstOrDefault(o => o.ID == dto.OrderID); // lookup navigation object
      OrderPayment payment = new OrderPayment
      {
        ID = dto.ID,
        Order = order,
        OrderID = dto.OrderID,
        Amount = dto.Amount,
        Date = (DateTimeOffset)dto.Date
      };
      OrderPayments.Add(payment);
      _context.OrderPayments.Add(payment);
      _context.SaveChanges();
    }
  }
}
