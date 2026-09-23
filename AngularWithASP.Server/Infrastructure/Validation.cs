using PCRE;

namespace AngularWithASP.Server.Infrastructure
{
  public static class MyRegex
  {
    // Restrictive regex for filtering the Orders Backlog
    public static string BacklogSearchOkayRegex{ get { return
        @"^[a-zA-Z0-9\-\.\@\_\+\:\/\s]{0,250}$"
    ;}}

    // String representation of a Guid
    public static string AppUserOrGuestId{ get { return
       @"^[a-zA-Z0-9]{8}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{12}$"
    ;}}

    // String representation of a Google Subject ID
    public static string GoogleSubject{ get { return
        "^[0-9]{20,255}$"
    ;}}

    public const string AddressLine1              = @"^[a-zA-Z0-9\s\/.,#-]{1,100}$"; // Must have one character
    public const string AddressLineN              = @"^[a-zA-Z0-9\s\/.,#-]{0,100}$"; // Can be empty
    public const string AddressCityStateOrCountry = @"^[a-zA-Z0-9\s.,-]{2,50}$";
    public const string AddressZipOrPostcode      = @"^\b\d{4}\b$"; // Exactly 4 digits
  }

  public static class PcreValidation
  {
    // Extend string class with a validation method that checks the given string against a given regex.
    // Validate the given string against a regex, using .NET PCRE
    // We are only looking for malicious input.
    // Null or empty strings are valid.
    public static bool ValidString(this string? input, string validationPattern)
    {
      if (!string.IsNullOrEmpty(input))
      {
        var regex = new PcreRegex(validationPattern);
        bool isValid = regex.IsMatch(input);
        if (!isValid)
        {
          return false; // Does not match the given regex
        }
      }
      // Matches the given regex -OR- input string was null or empty. (not an attack)
      return true;
    }
  }
}
