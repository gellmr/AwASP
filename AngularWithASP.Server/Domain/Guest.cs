using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AngularWithASP.Server.Domain
{
  public class GuestUpdateException : Exception{
    public GuestUpdateException(string? message) : base(message) { }
    public GuestUpdateException(string? message, Exception? inner) : base(message, inner) { }
    public Guest? Original { get; set; }
  }

  public class Guest
  {
    // Default constructor
    public Guest(){ }

    // Copy constructor
    public Guest(Guest g) {
      ID        = g.ID;
      Email     = g.Email;
      FirstName = g.FirstName;
      LastName  = g.LastName;
      Picture   = g.Picture;
    }

    [Key]
    public Guid ID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Picture { get; set; }
    public virtual IList<UOrder> Orders { get; set; }

    [NotMapped]
    public string FullName {
      get {
        if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)){
          return string.Empty; // Both empty
        }
        else if (string.IsNullOrEmpty(LastName)){ return FirstName; // FirstName available
        }
        else if (string.IsNullOrEmpty(FirstName)){ return LastName; // LastName available
        }
        else{ return FirstName + " " + LastName; // Both available.
        }
      }
    }
  }
}
