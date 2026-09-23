
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Collections.Immutable;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  internal class HomePageWaterSportCat : PageTest
  {
    public const string waterSportCatButtonCss = ".mg-category-menu-r a.btn[href='/category/waterSport']";
    public const string bottleThumbCss = ".mgImgThumb img[src=\"/thumbs/tilt-bottle.png\"]";
    
    [Test]
    public void ClickOnWaterSport_NavigatesToCorrectPage()
    {
      driver.Navigate().GoToUrl(viteUrl);
      const string buttonCss = waterSportCatButtonCss;
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(9));
        wait.Until(ExpectedConditions.ElementExists(By.CssSelector(buttonCss)));
        IReadOnlyCollection<IWebElement> waterSportLinks = driver.FindElements(By.CssSelector(buttonCss));
        List<IWebElement> links = waterSportLinks.ToList(); // There are 3 sizes used at different bootstrap breakpoints
        IWebElement smallLink = links[0];
        IWebElement medLink = links[1];
        IWebElement largeLink = links[2];
        IWebElement waterSportLink = wait.Until(ExpectedConditions.ElementToBeClickable(largeLink));
        waterSportLink.Click();
        // The thumbnail should take longest to load so wait for this...
        IWebElement drinkBottle = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(bottleThumbCss)));
        Assert.That(driver.Url, Does.Contain("/category/waterSport"));
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail("Timed out waiting for elements or page navigation.");
      }
    }
  }
}

