using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class MyOrdersTests : CheckoutTests
  {
    [Test]
    public void EmptyMyOrdersPage_LoadsSuccessfully()
    {
      driver.Navigate().GoToUrl(viteUrl + "/myorders");
      string titleElement = ".shopLayoutTransparent h2";
      string noOrdersElement = ".ordersEmptyMsg";
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(titleElement))); // Wait for element to be visible on page.
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail(pageOrElementMissing);
      }
      IWebElement element = driver.FindElement(By.CssSelector(titleElement));
      IWebElement noOrdElement = driver.FindElement(By.CssSelector(noOrdersElement));
      Assert.That(element.Text,       Does.Contain("My Orders"),             "My Orders page - title is incorrect.");
      Assert.That(noOrdElement.Text,  Does.Contain("(None at the moment)"),  "My Orders page - No Orders Message is incorrect.");
    }

    [Test]
    public void SubmitAutofill1_AppearsInMyOrders()
    {
      driver.Navigate().GoToUrl(viteUrl);
      AddBottleToCart();
      GoToCheckout();
      SubmitAutofill(1);

      // Go to My Orders... Should see Drink Bottle
      GoToMyOrders();
      ShouldSee_JohnDoe_BottleOrder();

      // Go to Order Detail ...  Should see Drink Bottle Details
      GoToMyOrderDetail();
      ShouldSee_JohnDoe_BottleOrdDetails();

      // Login as VIP... Should see (VIP user) Drink Bottle order in Backlog
      GoToLoginPage();
      LoginAsVip();
      ShouldSee_VipUser_BottleOrdBacklog();
    }
  }
}
