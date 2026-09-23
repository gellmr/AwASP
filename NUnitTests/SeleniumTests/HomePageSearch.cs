using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  internal class HomePageSearch : PageTest
  {
    public const string searchResultsAreaCss = ".shopLayoutTransparent";

    [Test]
    public void TypeSearch_BringsProductResults()
    {
      IWebElement? product1 = null;
      IWebElement? product2 = null;
      driver.Navigate().GoToUrl(viteUrl);
      const string searchCss = "input[placeholder='Search for products']";
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        IWebElement searchElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(searchCss)));
        searchElement.SendKeys("fla");

        string? expectedText1 = "Flat packed 30,000 seat stadium.";
        string? expectedText2 = "Corner Flags $25";
        
        // Use TextToBePresentInElementLocated to avoid Stale Element Reference exceptions
        // during React's asynchronous DOM updates
        wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector(searchResultsAreaCss), expectedText1));
        wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector(searchResultsAreaCss), expectedText2));

        IReadOnlyCollection<IWebElement> products = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(".productDetails")));
        List<IWebElement> prods = products.ToList();
        if (prods.Count < 2){
          Assert.Fail("BringsProductResults - Expected at least two products, but found less.");
        }
        product1 = prods[0];
        product2 = prods[1];
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail("BringsProductResults - timeout occurred.");
      }
      if (product1 == null){ Assert.Fail("BringsProductResults - product1 did not appear."); return; }
      if (product2 == null) { Assert.Fail("BringsProductResults - product2 did not appear."); return; }
      Assert.That(product1.Text, Does.Contain("Soccer Stadium $80000"), "BringsProductResults - First product - is missing.");
      Assert.That(product2.Text, Does.Contain("Corner Flags $25"), "BringsProductResults - Second product - is missing.");
    }
  }
}

