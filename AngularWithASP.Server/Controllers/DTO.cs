using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AngularWithASP.Server.DTO
{
  public class GuestUpdateDTO
  {
    public Guid ID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Picture { get; set; }
  }

  public class CartUpdateDTO
  {
    public Int32? cartLineID { get; set; }
    public Int32 qty { get; set; }
    public IspDTO? isp { get; set; }
  }

  public class IspDTO
  {
    public IspDTO(){
      id          = 0;
      title       = string.Empty;
      description = string.Empty;
      price       = 0.0M;
      category    = 0;
      image       = string.Empty;
    }
    public Int32 id { get; set; }

    [RegularExpression(@"^[A-Za-z0-9\s\-\.\,\(\)\+\?\:]{1,50}$", ErrorMessage = "Please use alphanumeric, spaces, dashes, period, comma, plus symbol, question mark, colon, parentheses, 1-50 characters")]
    public string title { get; set; }

    [RegularExpression(@"^[A-Za-z0-9\s\-\.\,\(\)\+\?\:]{1,130}$", ErrorMessage = "Please use alphanumeric, spaces, dashes, period, comma, plus symbol, question mark, colon, parentheses, 1-130 characters")]
    public string description { get; set; }

    public decimal price { get; set; }
    public Int32 category { get; set; }

    public string image { get; set; }
  }

  public class CartSubmitLineDTO { public Int32? cartLineID { get; set; } public Int32 qty { get; set; } public IspDTO? isp { get; set; } }

  namespace MyOrders
  {
    public class OrderIdRequest
    {
      public Int32 orderID { get; set; }
    }

    public class UserIdDTO
    {
      public string? uid { get; set; }
      public Guid? gid { get; set; }
    }
  }

  public class CheckoutSubmitDTO
  {
    [Required(ErrorMessage = "Please enter your First Name")]
    [RegularExpression(OkInputs.CheckoutName50, ErrorMessage = OkInputs.CheckoutName50Err)]
    public string? FirstName { get; set; }

    //[Required(ErrorMessage = "Please enter your Last Name")]
    [RegularExpression(OkInputs.CheckoutName50, ErrorMessage = OkInputs.CheckoutName50Err)]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Please enter the first address line")]
    [RegularExpression(OkInputs.CheckoutShip100, ErrorMessage = OkInputs.CheckoutShip100Err)]
    public string? ShipLine1 { get; set; }

    [RegularExpression(OkInputs.CheckoutShip100, ErrorMessage = OkInputs.CheckoutShip100Err)]
    public string? ShipLine2 { get; set; }

    [RegularExpression(OkInputs.CheckoutShip100, ErrorMessage = OkInputs.CheckoutShip100Err)]
    public string? ShipLine3 { get; set; }

    [Required(ErrorMessage = "Please enter a city name")]
    [RegularExpression(OkInputs.CheckoutDetail50, ErrorMessage = OkInputs.CheckoutDetail50Err)]
    public string? ShipCity { get; set; }

    [Required(ErrorMessage = "Please enter a state name")]
    [RegularExpression(OkInputs.CheckoutDetail50, ErrorMessage = OkInputs.CheckoutDetail50Err)]
    public string? ShipState { get; set; }

    [Required(ErrorMessage = "Please enter a country name")]
    [RegularExpression(OkInputs.CheckoutDetail50, ErrorMessage = OkInputs.CheckoutDetail50Err)]
    public string? ShipCountry { get; set; }

    [Required(ErrorMessage = "Please enter your postcode")]
    [RegularExpression(OkInputs.CheckoutZip, ErrorMessage = OkInputs.CheckoutZipErr)]
    public string? ShipZip { get; set; }

    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(OkInputs.Email, ErrorMessage = OkInputs.EmailErr)]
    public string? ShipEmail { get; set; }

    public Guid? guestID { get; set; }
    public List<CartSubmitLineDTO>? cart { get; set; }
  }

  public class ConfirmGoogleAuthDTO
  {
    [Required(ErrorMessage = "clientId is required")]
    public string? clientId { get; set; } // eg about 72 characters long "08mvd3ih...a3m7c6t.apps.googleusercontent.com"

    [Required(ErrorMessage = "credential is required")]
    public string? credential { get; set; } // eg about 998 characters long. Has 3 sections separated by period (.) Eg, "eyJhbGciOiJSUzI1NiIs.ImtpZCI6Ijg4MjUwM2E1.ZmQ1NmU5ZjczNGRmYmE1Y"

    [Required(ErrorMessage = "select_by is required")]
    public string? select_by { get; set; } // eg about 3 characters long "btn"
  }

  public class AdminLoginSubmitDTO
  {
    [Required(ErrorMessage = "username is required")]
    public string? username { get; set; }

    [Required(ErrorMessage = "password is required")]
    public string? password { get; set; }
  }

  public class GoogleAppUserDTO
  {
    [Required(ErrorMessage = "Subject is required")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "GivenName is required")]
    public string? GivenName { get; set; }

    [Required(ErrorMessage = "FamilyName is required")]
    public string? FamilyName { get; set; }

    [Required(ErrorMessage = "FullName is required")]
    public string? FullName { get; set; }

    public string? Picture { get; set; }

    public string? UserName { get; set; } // Generated, eg "diana-walters-e35"
  }

  public class OrderSlugDTO // Used to render rows on the Admin Orders page.
  {
    public string? ID { get; set; }
    public Int32 RowNumber { get; set; }
    public string? Username { get; set; }
    public string? UserID { get; set; }
    public string? GuestID { get; set; }
    public string? UserIDshort { get; set; } // To display in table cell
    public string? GuestIDshort { get; set; } // To display in table cell
    public string? AccountType { get; set; }
    public string? Email { get; set; }
    public string? OrderPlacedDate { get; set; }
    public string? PaymentReceivedAmount { get; set; }
    public string? Outstanding { get; set; }
    public string? ItemsOrdered { get; set; }
    public string? Items { get; set; }
    public string? OrderStatus { get; set; }
    public Int32 TotalRows { get; set; }
  }

  namespace OrderDTOs
  {
    public class AddressDTO
    {
      public string? Line1 { get; set; }
      public string? Line2 { get; set; }      
      public string? Line3 { get; set; }
      public string? City { get; set; }
      public string? State { get; set; }
      public string? Country { get; set; }
      public string? Zip { get; set; }
    }
  }

  namespace AdminUserAccounts
  {
    public class UserDTO // Cut back details of an AppUser object.
    {
      public string? Email { get; set; }
      public bool? EmailConfirmed { get; set; }
      
      public Guid? GuestID { get; set; }
      public string? Id { get; set; }
      
      // Orders
      public string? PhoneNumber { get; set; }
      public bool? PhoneNumberConfirmed { get; set; }
      public string? Picture { get; set; }
      public string? UserName { get; set; }
      public string? FullName { get; set; }

      public static UserDTO? TryParse(Guest? g)
      {
        return (g == null) ? null : new UserDTO{
          Id = null,
          GuestID = g.ID,
          Email = g.Email,
          Picture = g.Picture,
          UserName = MyExtensions.GenUserName(g.FullName, g.ID.ToString()),
          FullName = g.FullName
        };
      }

      public static UserDTO TryParse(AppUser u)
      {
        return new UserDTO
        {
          Email = u.Email,
          EmailConfirmed = u.EmailConfirmed,
          GuestID = u.GuestID,
          Id = u.Id,
          PhoneNumber = u.PhoneNumber,
          PhoneNumberConfirmed = u.PhoneNumberConfirmed,
          Picture = u.Picture,
          UserName = u.UserName,
          FullName = u.FullName
        };
      }
    }
  }

  namespace RandomUserme
  {
    public class NameDTO{
      public string? Title{ get; set; } public string? First { get; set; } public string? Last { get; set; }
    }
    public class StreetDTO {
      public Int32? Number { get; set; } public string? Name { get; set; }
    }
    public class PictureDTO{
      public string? Large { get; set; } public string? Medium { get; set; } public string? Thumbnail { get; set; }
    }
    public class CoordsDTO{
      public string? Latitude { get; set; } public string? Longitude { get; set; }
    }
    public class TimeZoneDTO{
      public string? Offset { get; set; } public string? Description { get; set; }
    }
    public class  LocationDTO{
      public StreetDTO? Street { get; set; }
      public string? City { get; set; }
      public string? State { get; set; }
      public string? Country { get; set; }
      public Int32? PostCode { get; set; }
      public CoordsDTO? Coordinates { get; set; }
      public TimeZoneDTO? TimeZone {get; set; }
    }
    public class UserDTO{
      public string? Gender { get; set; }
      public NameDTO? Name { get; set; }
      public LocationDTO? Location { get; set; }
      public string? Email { get; set; }
      public string? Phone { get; set; }
      public PictureDTO? Picture { get; set; }
    }
    public class InfoDTO
    {
      public string? Seed {get; set;}
      public Int32? Results { get; set; }
      public Int32? Page { get; set; }
      public string? Version { get; set; }
    }
    public class ResponseDTO
    {
      public List<UserDTO>? Results { get; set; }
      public InfoDTO? Info { get; set; }
    }
  }
}
