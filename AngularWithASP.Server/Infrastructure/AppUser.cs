using Microsoft.AspNetCore.Identity;
using AngularWithASP.Server.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularWithASP.Server.Infrastructure
{
  public class AppUser: IdentityUser
  {
    public virtual ICollection<UOrder> Orders { get; set; }

    [ForeignKey("GuestID")]
    public virtual Guest Guest { get; set; } // Navigation property.
    public Guid? GuestID { get; set; }

    public string? Picture { get; set; } // Fetched from randomuser.me during seed on start

    public string? FullName { get; set; } // The display name. Custom property can contain spaces.

    public void updateFullName(string? fullName)
    {
      if (string.IsNullOrWhiteSpace(fullName)){
        throw new ArgumentException("Empty name");
      }
      if (string.IsNullOrEmpty(fullName)){
        throw new ArgumentException("Empty name");
      }
      FullName = fullName;
    }

    public static string GetFirstName(string fullName)
    {
      if (string.IsNullOrWhiteSpace(fullName)) { return string.Empty; }
      string[] names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      if (names.Length > 0) { return names[0]; }
      return string.Empty;
    }

    public static string GetLastName(string fullName)
    {
      if (string.IsNullOrWhiteSpace(fullName)) { return string.Empty; }
      string[] names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      if (names.Length > 1) { Int32 lastIdx = names.Length - 1; return names[lastIdx]; }
      return string.Empty;
    }
  }
}
