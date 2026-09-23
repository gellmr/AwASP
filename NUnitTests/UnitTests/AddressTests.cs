using Microsoft.Identity.Client;
using AngularWithASP.Server.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests.UnitTests
{
  [TestFixture]
  public class AddressTests
  {
    [Test]
    public void GetDefault_ShouldBeValid(){
      Address address = AddressGen.GetDefault();
      ShouldBeValid(address);
    }

    [Test]
    public void GetValidWithRegexLimit_ShouldBeValid(){
      Address address = AddressGen.GetValidWithRegexLimit();
      ShouldBeValid(address);
    }

    [Test]
    public void GetDefault_WithLine1Err_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.Line1 = address.Line1 + "<"; // illegal character
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetDefault_WithEmptyLine1_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.Line1 = string.Empty;
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetInvalidWithXss_ShouldNotBeValid(){
      Address addressWithXss = AddressGen.GetInvalidWithXss();
      ShouldNotBeValid(addressWithXss);
    }

    [Test]
    public void GetDefaultWithPostCodeTooShort_ShouldNotBeValid()
    {
      Address address = AddressGen.GetDefault();
      address.Zip = "123";
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetDefaultWithPostCodeTooLong_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.Zip = "12345";
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetDefaultWithLine2And3_ShouldBeValid(){
      Address address = AddressGen.GetDefault();
      address.Line2 = AddressGen.validLine;
      address.Line3 = AddressGen.validLine;
      ShouldBeValid(address);
    }

    [Test]
    public void GetDefaultWithLine2And3Null_ShouldBeValid(){
      Address address = AddressGen.GetDefault();
      address.Line2 = null;
      address.Line3 = null;
      ShouldBeValid(address);
    }
    [Test]
    public void GetDefaultWithLine2And3Empty_ShouldBeValid(){
      Address address = AddressGen.GetDefault();
      address.Line2 = string.Empty;
      address.Line3 = string.Empty;
      ShouldBeValid(address);
    }

    [Test]
    public void GetDefaultWithCityNull_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.City = null;
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetDefaultWithStateNull_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.State = null;
      ShouldNotBeValid(address);
    }

    [Test]
    public void GetDefaultWithCountryNull_ShouldNotBeValid(){
      Address address = AddressGen.GetDefault();
      address.Country = null;
      ShouldNotBeValid(address);
    }

    // Reusable helper method
    public void ShouldBeValid(Address address)
    {
      try
      {
        // Arrange
        var validationContext = new ValidationContext(address, null, null);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(address, validationContext, validationResults, true);

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(validationResults, Is.Empty);
      }
      catch (Exception ex)
      {
        Console.WriteLine("ShouldBeValid() - An error occurred: " + ex.Message);
      }
    }

    // Reusable helper method
    public void ShouldNotBeValid(Address address)
    {
      try
      {
        // Arrange
        var validationContext = new ValidationContext(address, null, null);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(address, validationContext, validationResults, true);

        // Assert
        Assert.That(isValid, Is.False);
        Assert.That(validationResults, Is.Not.Empty);
      }
      catch (Exception ex)
      {
        Console.WriteLine("ShouldNotBeValid() - An error occurred: " + ex.Message);
      }
    }
  }

  // This class generates various Address instances for testing purposes.
  public static class AddressGen
  {
    public const string? validLine = "Unit 1/100, 3-3 #44 Main St."; // Uses all valid characters

    // This is a typical valid address.
    public static Address GetDefault()
    {
      return new Address
      {
        Line1 = AddressGen.validLine,
        Line2 = null,
        Line3 = null,
        City = "Anytown",
        State = "WA",
        Country = "Australia",
        Zip = "1234"
      };
    }

    // This address uses all valid characters, and is very long, but still valid.
    // It is designed to test the limits of the regex.
    public static Address GetValidWithRegexLimit()
    {
      return new Address
      {
        Line1 = "P.O. Box 45-A,    ////....----####abcdefgabcdefgabcdefgabcdefgabcdefgabcdefgabcdefgabcdefgabcdefg100",
        City = "The 1000 Island Province - Island 33, Peninsula 2.",
        State = "The 1000 Island Province - Island 33, Peninsula 2.",
        Country = "The 1000 Island Province - Island 33, Peninsula 2.",
        Zip = "9999"
      };
    }

    // This address contains XSS attack code in every field, which should be caught by validation.
    // It is designed to check that a typical XSS attack is caught by the regex.
    public static Address GetInvalidWithXss()
    {
      string? xss = "<script>alert('XSS');</script>";
      return new Address
      {
        Line1 = xss,
        Line2 = xss,
        Line3 = xss,
        City = xss,
        State = xss,
        Country = xss,
        Zip = xss
      };
    }
  }
}
