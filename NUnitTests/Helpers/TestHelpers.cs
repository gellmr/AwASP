using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace NUnitTests.Helpers
{
  public static class TestHelpers
  {
    public static void scrollTo(this IWebDriver driver, string selector)
    {
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      js.ExecuteScript("document.querySelector(arguments[0]).scrollIntoView({block: 'center', behavior: 'smooth'});", selector);
      wait.Until(CustomExpectedConditions.ElementInViewport(By.CssSelector(selector)));
      wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
    }

    public static string TrimAndFlattenString(string? input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }
      else
      {
        // Replace all newline characters with a single space
        string noNewlines = input.Replace("\r\n", " ")
                                 .Replace("\n", " ")
                                 .Replace("\r", " ");

        // Replace multiple spaces with a single space
        // This uses a loop to handle cases where there might be more than two spaces
        while (noNewlines.Contains("  ")) // Two spaces
        {
          noNewlines = noNewlines.Replace("  ", " ");
        }

        // Trim leading and trailing whitespace
        return noNewlines.Trim();
      }
    }
  }
}
