using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using NUnitTests.Helpers;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  internal class HomePageSoccerCat : PageTest
  {
    public const string soccerGoalsThumbCss = ".mgImgThumb img[src=\"/thumbs/goals1.png\"]";
    public const string soccerRowCss = ".inStockProductCanAdd";
    
    [Test]
    public void ClickOnSoccer_NavigatesToCorrectPage()
    {
      driver.Navigate().GoToUrl(viteUrl);
      const string buttonCss = ".mg-category-menu-r a.btn[href='/category/soccer']";
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(9));
        wait.Until(ExpectedConditions.ElementExists(By.CssSelector(buttonCss)));
        IReadOnlyCollection<IWebElement> catLinks = driver.FindElements(By.CssSelector(buttonCss));
        List<IWebElement> links = catLinks.ToList(); // There are 3 sizes used at different bootstrap breakpoints
        IWebElement smallLink = links[0];
        IWebElement medLink = links[1];
        IWebElement largeLink = links[2];
        // Need to make sure we are on a large screen size, or clicking the large link will not work.
        IWebElement clickableLink = wait.Until(ExpectedConditions.ElementToBeClickable(largeLink));
        clickableLink.Click();
        // The thumbnail should take longest to load so wait for this...
        IWebElement soccerGoals = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(soccerGoalsThumbCss)));
        IReadOnlyCollection<IWebElement> soccerRows = driver.FindElements(By.CssSelector(soccerRowCss));
        List<IWebElement> rows = soccerRows.ToList();
        IWebElement row1 = rows[0];
        string? soccerText = TestHelpers.TrimAndFlattenString(row1.Text);
        Assert.That(driver.Url, Does.Contain("/category/soccer"), "Failed to reach soccer category");
        Assert.That(soccerText, Does.Contain("Soccer Goals $1000"), "First product - Title is incorrect.");
        Assert.That(soccerText, Does.Contain("One lightweight aluminium standard size impact foam coated soccer goal with netting."), "First product - Description is incorrect.");
        Assert.That(soccerText, Does.Contain("Add to Cart"), "First product - Controls are incorrect.");
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail("Timed out waiting for elements or page navigation.");
      }
    }
  }
}

