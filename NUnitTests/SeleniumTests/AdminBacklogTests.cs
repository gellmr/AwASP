using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class AdminBacklogTests : AdminTest
  {
    public string? pagLink2Css = "a[href=\"/admin/orders/2\"]";
    public string? backLogRowCss = "tr.backlogCursorRow";

    [Test]
    public void BacklogPage_PaginationShouldWork()
    {
      GoToBackLog();
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        By elementsLocator = By.CssSelector(backLogRowCss);

        // Safely fetch Page 1 elements with retry logic for stability
        IList<IWebElement> allElements = wait.Until(d => {
          try {
            var rows = d.FindElements(elementsLocator);
            return (rows.Count == 12) ? rows : null;
          } catch (StaleElementReferenceException) {
            return null;
          }
        });
        Assert.That(allElements.Count, Is.EqualTo(12), "BacklogPage_PaginationShouldWork - Page 1 does not have 12 rows");

        // Grab the first row's "#Result" column text (which should be "1")
        IWebElement firstRowResultCol = allElements.First().FindElement(By.CssSelector("td:first-child"));
        string firstRowResult = firstRowResultCol.Text.Trim();
        Assert.That(firstRowResult, Is.EqualTo("1"), "BacklogPage_PaginationShouldWork - Page 1 does not start with Result #1");

        // Grab all Order IDs on Page 1 to ensure they don't overlap with Page 2
        var page1OrderIds = allElements.Select(r => r.GetAttribute("data-orderid")).ToList();

        IWebElement clickableButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(pagLink2Css)));
        clickableButton.Click();

        wait.Until(d => {
          try 
          {
            var rows = d.FindElements(elementsLocator);
            if (rows.Count == 0) return false;
        
            var firstCell = rows.First().FindElement(By.CssSelector("td:first-child"));
            return firstCell.Text.Trim() == "13";
          }
          catch (StaleElementReferenceException)
          {
            // React is mid-render; ignore and let WebDriverWait retry
            return false;
          }
          catch (NoSuchElementException)
          {
            // The <td> hasn't mounted yet inside the row; ignore and retry
            return false;
          }
        });

        allElements = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(elementsLocator));
        Assert.That(allElements.Count, Is.EqualTo(12), "BacklogPage_PaginationShouldWork - Page 2 does not have 12 rows");

        var page2OrderIds = allElements.Select(r => r.GetAttribute("data-orderid")).ToList();

        // Ensure no overlap between pages
        var intersection = page1OrderIds.Intersect(page2OrderIds);
        Assert.That(intersection.Count(), Is.EqualTo(0), "BacklogPage_PaginationShouldWork - Page 1 and Page 2 contain overlapping orders!");
      }
      catch (WebDriverTimeoutException){
        Assert.Fail("BacklogPage_PaginationShouldWork - Timeout occurred");
      }
    }

    public void GoToBackLog()
    {
      driver.Navigate().GoToUrl(viteUrl);
      GoToLoginPage();
      LoginAsVip();
    }
  }
}
