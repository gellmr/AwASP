using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;
using System.Runtime;

// Add Services to the DI container.
var builder = WebApplication.CreateBuilder(args);
IHostEnvironment env = builder.Environment;

// Load either Development or Production JSON config. (Not in source control)
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Configure SQL Server to always use an execution strategy with retries.
builder.Services.AddDbContext<StoreContext>(options =>
  options.UseSqlServer(
    builder.Configuration.GetConnectionString("StoreContext"),
    sqlServerOptionsAction: sqlOptions =>
    {
      sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null); // You can specify specific error numbers here
    })
);

builder.Services.AddIdentity<AppUser, IdentityRole>(options => { 
  options.SignIn.RequireConfirmedAccount = true;
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Default is 5 minutes
  options.Lockout.AllowedForNewUsers = true; // "Opt in" to using the lockout functionality. True does not mean the user is locked out.
})
.AddEntityFrameworkStores<StoreContext>()
.AddSignInManager<SignInManager<AppUser>>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly   = true; // Prevents client-side script access
  options.ExpireTimeSpan    = TimeSpan.FromMinutes(60);
  options.LoginPath         = "/admin";
  options.AccessDeniedPath  = "/admin";
  options.SlidingExpiration = true;
  options.Cookie.Name = MyExtensions.IdentityCookieName;
});

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(options =>{
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
  options.Cookie.Name = MyExtensions.IdentityCookieName;
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register custom authorization middleware handler with DI container
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

// Random User Generator API to fetch a list of (generated) user data for development projects. Use seed to ensure same results.
builder.Services.AddHttpClient<RandomUserMeApiClient>(client =>{
  client.BaseAddress = new Uri(RandomUserMeApiClient.BaseAddress);
  client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// DI declarations
builder.Services.AddScoped<IOrdersRepository, EFOrdersRepository>();
builder.Services.AddScoped<ICartLineRepository, EFCartLineRepository>();
builder.Services.AddScoped<IInStockRepository, EFInStockRepository>();
builder.Services.AddScoped<IGuestRepository, EFGuestRepository>();
builder.Services.AddScoped<StoreContext, StoreContext>();
builder.Services.AddScoped<MyEnv, MyEnv>();
builder.Services.AddScoped<CustomMigrator, CustomMigrator>();
builder.Services.AddScoped<DataSeeder, DataSeeder>();

// -------------------------------------------------------------

// For GCP Cloud Run: Read the PORT environment variable dynamically assigned to the container
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
  builder.WebHost.UseUrls($"http://*:{port}");
}

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();
app.MapControllers();

// For GCP Cloud Run: Only execute migrations and seeder if explicitly requested.
// We don't want Cloud Run container cold-starts trying to migrate the DB simultaneously.
bool runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS") == "true" || app.Environment.IsDevelopment();

if (runMigrations)
{
  using (var scope = app.Services.CreateScope()){
    var services = scope.ServiceProvider;
    var custMigrator = services.GetRequiredService<CustomMigrator>();
    await custMigrator.Execute();
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.Execute();
  }
}
app.MapFallbackToFile("/index.html");
app.Run();
