using NUnit.Framework;
using NUnitTests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net.NetworkInformation;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class CheckoutTests: AdminTest
  {
    public string titleElement = ".shopLayoutTransparent h2";
    public string coSubmit = ".checkoutSubmitBtnGroup button[type=\"submit\"]";
    public string autoBtn = ".checkoutSubmitBtnGroup button[type=\"button\"]";
    public string validationElement = ".error";
    public string fieldElement = "input.form-control";
    //public string coSuccess = "#noShopLayout .shopLayoutTransparent";
    public string coSuccess = "#shopLayout .shopLayoutTransparent";
    public string coThanks = "#shopLayout .checkoutThanks";

    public IWebElement? v_firstName = null;
    public IWebElement? v_lastName = null;
    public IWebElement? v_line1 = null;
    public IWebElement? v_line2 = null;
    public IWebElement? v_line3 = null;
    public IWebElement? v_city = null;
    public IWebElement? v_state = null;
    public IWebElement? v_country = null;
    public IWebElement? v_zip = null;
    public IWebElement? v_email = null;

    public string? s_firstName = null;
    public string? s_lastName = null;
    public string? s_line1 = null;
    public string? s_line2 = null;
    public string? s_line3 = null;
    public string? s_city = null;
    public string? s_state = null;
    public string? s_country = null;
    public string? s_zip = null;
    public string? s_email = null;

    public string? c_firstName = null;
    public string? c_lastName = null;
    public string? c_line1 = null;
    public string? c_line2 = null;
    public string? c_line3 = null;
    public string? c_city = null;
    public string? c_state = null;
    public string? c_country = null;
    public string? c_zip = null;
    public string? c_email = null;

    public void GoToCheckout()
    {
      driver.Navigate().GoToUrl(viteUrl + "/checkout");
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
      IWebElement coTitleElement = null;
      try
      {
        coTitleElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(titleElement)));
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail(pageOrElementMissing); return;
      }
      //IWebElement coTitleElement = driver.FindElement(By.CssSelector(titleElement));
      Assert.That(coTitleElement.Text, Does.Contain("Checkout"), "Checkout - title is incorrect.");
    }

    public void GetValidations(List<IWebElement> vals)
    {
      v_firstName = vals[0];
      v_lastName = vals[1];
      v_line1 = vals[2];
      v_line2 = vals[3];
      v_line3 = vals[4];
      v_city = vals[5];
      v_state = vals[6];
      v_country = vals[7];
      v_zip = vals[8];
      v_email = vals[9];

      s_firstName = v_firstName.Text;
      s_lastName = v_lastName.Text;
      s_line1 = v_line1.Text;
      s_line2 = v_line2.Text;
      s_line3 = v_line3.Text;
      s_city = v_city.Text;
      s_state = v_state.Text;
      s_country = v_country.Text;
      s_zip = v_zip.Text;
      s_email = v_email.Text;
    }

    public void GetFields(List<IWebElement> fields)
    {
      c_firstName = fields[0].Text;
      c_lastName = fields[1].Text;
      c_line1 = fields[2].Text;
      c_line2 = fields[3].Text;
      c_line3 = fields[4].Text;
      c_city = fields[5].Text;
      c_state = fields[6].Text;
      c_country = fields[7].Text;
      c_zip = fields[8].Text;
      c_email = fields[9].Text;
    }

    public void SubmitAutofill(Int32 clickCount)
    {
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
      try
      {
        // Click N times with safety against stale elements and DOM re-renders
        for (int i = 0; i < clickCount; i++)
        {
          IWebElement autoButton = wait.Until(d =>
          {
            try
            {
              var element = d.FindElement(By.CssSelector(autoBtn));
              return (element.Displayed && element.Enabled) ? element : null;
            }
            catch (StaleElementReferenceException)
            {
              return null;
            }
          });
          IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
          js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'center', inline: 'nearest'});", autoButton);
          autoButton.Click();
        }

        // Wait for fields, extract them, and process
        wait.Until(d => d.FindElements(By.CssSelector(fieldElement)).Count > 0);
        IReadOnlyCollection<IWebElement> allFields = driver.FindElements(By.CssSelector(fieldElement));
        List<IWebElement> fields = allFields.ToList();
        GetFields(fields);

                IWebElement clickableButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(coSubmit)));
        clickableButton.Click();

        IWebElement coSuccessElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(coThanks)));
        string successText = TestHelpers.TrimAndFlattenString(coSuccessElement.Text);
        Assert.That(successText, Does.Contain("Thanks!"), "Checkout - success - Title - incorrect.");
        Assert.That(successText, Does.Contain("Your order has been submitted"),              "Checkout - success - Message - incorrect.");
        Assert.That(successText, Does.Contain("We'll ship your goods as soon as possible."), "Checkout - success - Message - incorrect.");
        Assert.That(successText, Does.Contain("Continue Shopping"), "Checkout - success - Button - incorrect.");
        Assert.That(driver.Url, Does.Contain("/checkoutsuccess"), "Failed to reach checkout success page.");

        myOrdNavBtn = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(myOrdCssBtn)));
        // Instead of waiting on the cached element, poll the locator until text appears
        wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector(myOrdCssBtn), "My Orders (1)"));
        // Re-fetch to ensure we have the latest DOM node for the final assertion
        myOrdNavBtn = driver.FindElement(By.CssSelector(myOrdCssBtn));
      }
      catch (WebDriverTimeoutException ex)
      {
        Assert.Fail("Timeout during SubmitAutofill " + ex.Message);
      }
      Assert.That(myOrdNavBtn.Text, Does.Contain("My Orders (1)"), "Checkout - success - My Orders Button - incorrect.");
    }

    [Test]
    public void EmptyCheckoutPage_LoadsSuccessfully(){
      GoToCheckout();
    }

    [Test]
    public void SubmitEmpty_ShowsClientValidation()
    {
      GoToCheckout();
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
      try{
        IWebElement submitButton = wait.Until(d =>
        {
          try
          {
            var element = d.FindElement(By.CssSelector(coSubmit));
            return (element.Displayed && element.Enabled) ? element : null;
          }
          catch (StaleElementReferenceException)
          {
            return null;
          }
        });
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'center', inline: 'nearest'});", submitButton);
        submitButton.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(validationElement)));
        IReadOnlyCollection<IWebElement> validations = driver.FindElements(By.CssSelector(validationElement));
        List<IWebElement> vals = validations.ToList();
        GetValidations(vals);
      }
      catch(WebDriverTimeoutException ex){
        Assert.Fail("Timeout during SubmitEmpty_ShowsClientValidation");
      }
      Assert.That(s_firstName, Does.Contain("First Name is required."), "Checkout - first name - validation - msg incorrect.");
      Assert.That(s_lastName,  Does.Contain(""),                        "Checkout - last name  - validation - msg incorrect.");

      Assert.That(s_line1, Does.Contain("Address Line 1 is required."), "Checkout - line 1     - validation - msg incorrect.");
      Assert.That(s_line2, Does.Contain(""),                            "Checkout - line 2     - validation - msg incorrect.");
      Assert.That(s_line3, Does.Contain(""),                            "Checkout - line 3     - validation - msg incorrect.");

      Assert.That(s_city,    Does.Contain("City is required."),    "Checkout - city    - validation - msg incorrect.");
      Assert.That(s_state,   Does.Contain("State is required."),   "Checkout - state   - validation - msg incorrect.");
      Assert.That(s_country, Does.Contain("Country is required."), "Checkout - country - validation - msg incorrect.");
      Assert.That(s_zip,     Does.Contain("Zip is required."),     "Checkout - zip     - validation - msg incorrect.");

      Assert.That(s_email, Does.Contain("Email is required."),     "Checkout - email   - validation - msg incorrect.");
    }

    [Test]
    public void SubmitAutofill1_ShowsCheckoutSuccess()
    {
      driver.Navigate().GoToUrl(viteUrl);
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
      try { AddBottleToCart(); }
      catch (WebDriverTimeoutException) { Assert.Fail(pageOrElementMissing); }
      GoToCheckout();
      SubmitAutofill(1);
    }
  }
}
