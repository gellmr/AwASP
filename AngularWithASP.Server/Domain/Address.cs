using System.ComponentModel.DataAnnotations;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Domain
{
  
  public class Address
  {
    [Key]
    public Int32 ID { get; set; }

    [Display(Name = "Line 1")]
    [Required(ErrorMessage = "Please enter the first address line")]
    [RegularExpression(MyRegex.AddressLine1, ErrorMessage = "Invalid characters in address line.")]
    public string? Line1 { get; set; }
    
    [Display(Name = "Line 2")]
    [RegularExpression(MyRegex.AddressLineN, ErrorMessage = "Invalid characters in address line.")]
    public string? Line2 { get; set; }

    [Display(Name = "Line 3")]
    [RegularExpression(MyRegex.AddressLineN, ErrorMessage = "Invalid characters in address line.")]
    public string? Line3 { get; set; }

    [Required(ErrorMessage = "Please enter the city, town or suburb")]
    [RegularExpression(MyRegex.AddressCityStateOrCountry, ErrorMessage = "Invalid city, town or suburb.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Please enter a state name")]
    [RegularExpression(MyRegex.AddressCityStateOrCountry, ErrorMessage = "Invalid state name.")]
    public string? State { get; set; }

    [Required(ErrorMessage = "Please enter a country name")]
    [RegularExpression(MyRegex.AddressCityStateOrCountry, ErrorMessage = "Invalid country name.")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Please enter the zip")]
    [RegularExpression(MyRegex.AddressZipOrPostcode, ErrorMessage = "Invalid zip or postal code format.")]
    public string? Zip { get; set; }
  }
}
