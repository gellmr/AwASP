using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class HomepageTests : PageTest
  {
    // The [Test] attribute identifies a test method.
    // This test assumes the servers are already running, thanks to the [SetUpFixture].
    [Test]
    public void HomePage_LoadsSuccessfully()
    {
      driver.Navigate().GoToUrl(viteUrl);
      try{
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a.storeBrand"))); // Wait for the heading to be present and visible on the page.
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".productDetails")));
      }
      catch (WebDriverTimeoutException){
        Assert.Fail(pageOrElementMissing);
      }
      IWebElement heading = driver.FindElement(By.CssSelector("a.storeBrand"));
      IReadOnlyCollection<IWebElement> products = driver.FindElements(By.CssSelector(".productDetails"));

      Assert.That(driver.Title,          Does.Contain("Shop"),             "Home page - title is incorrect.");
      Assert.That(heading.Text,          Is.EqualTo("SPORTS STORE"),       "Heading text - is incorrect.");
      Assert.That(products.First().Text, Does.Contain("Drink Bottle $20"), "First product - is missing.");
      Assert.That(products.Count,        Is.EqualTo(4),                    "4 products - not found on page.");
    }
  }
}
