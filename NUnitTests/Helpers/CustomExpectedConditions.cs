using OpenQA.Selenium;

namespace NUnitTests.Helpers
{
  // Custom wait condition to ensure an element is within the viewport.
  public static class CustomExpectedConditions
  {
    public static Func<IWebDriver, bool> ElementInViewport(By locator)
    {
      return (driver) =>
      {
        try
        {
          var element = driver.FindElement(locator);
          var js = (IJavaScriptExecutor)driver;
          var rect = (Dictionary<string, object>)js.ExecuteScript("return arguments[0].getBoundingClientRect();", element);

          double top = Convert.ToDouble(rect["top"]);
          double bottom = Convert.ToDouble(rect["bottom"]);

          // Check if the element's top and bottom coordinates are within the viewport
          return top >= 0 && bottom <= (long)js.ExecuteScript("return window.innerHeight");
        }
        catch (WebDriverTimeoutException)
        {
          throw;
        }
        catch (NoSuchElementException)
        {
          return false;
        }
        catch (StaleElementReferenceException)
        {
          return false;
        }
      };
    }
  }
}
