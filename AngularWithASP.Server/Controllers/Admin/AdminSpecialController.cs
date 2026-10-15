using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AngularWithASP.Server.Infrastructure;
using AngularWithASP.Server.Domain.Abstract;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;

namespace AngularWithASP.Server.Controllers.Admin
{
  // Controller for http endpoints that do special 'developer only' administrative actions
  // such as reset the database to its seeded baseline.
  // These http endpoints can be hit by curl using the VIP password (not in source control).
  // The endpoints are not protected by Identity / Cookie auth so anyone can try and hit them.
  // For this reason the endpoints are protected by a middleware rate limiter.
  public class AdminSpecialController : AdminBaseController
  {
    private readonly IConfiguration _config;
    private readonly CustomMigrator _custMigrator;
    private readonly DataSeeder _seeder;
    private readonly StoreContext _context;
    protected string? _vipPassword;

    public class ResetRequestModel{
      public string Password { get; set; } = string.Empty;
    }

    public AdminSpecialController(
      UserManager<AppUser> u,
      IGuestRepository g,
      IConfiguration c,
      CustomMigrator custMigrator,
      DataSeeder seeder,
      StoreContext context
    ) : base(u, g)
    {
      _config = c;
      _custMigrator = custMigrator;
      _seeder = seeder;
      _context = context;
      _vipPassword = c.GetSection("Authentication:VIP:Password").Value;
    }

    /*
    Secure http endpoint to reset the database to its seeded baseline. Example usage:
    $body = @{ password = $env:Authentication__VIP__Password } | ConvertTo-Json;
    curl.exe -X POST https://site-url:PORT/api/admin-special/master-reset -H "Content-Type: application/json" -d $body
    */
    [HttpPost("admin-special/master-reset")] // POST "/api/admin-special/master-reset"
    [AllowAnonymous]
    [EnableRateLimiting("StrictRatePolicy")]
    public async Task<IActionResult> MasterReset([FromBody] ResetRequestModel model) // Bind model from JSON body
    {
      // 1. Ensure the given password matches the one from our configuration
      bool password_ok = !string.IsNullOrEmpty(_vipPassword) && SecurePasswordEquals.FixedTimeCompare(model?.Password, _vipPassword);
      if (!password_ok)
      {
        return Unauthorized(new { message = "Invalid master reset key." });
      }

      try
      {
        await _context.Database.EnsureDeletedAsync(); // 2. Drop the existing database
        await _custMigrator.Execute();                // 3. Re-run your custom migrator (creates tables, applies schema)
        await _seeder.Execute();                      // 4. Re-run your data seeder
        return Ok(new { message = "Database successfully wiped, migrated, and re-seeded." });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error resetting database", error = ex.Message });
      }
    }
  }
}