
using static System.Net.Mime.MediaTypeNames;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class AdminProductsTest : AdminTest
  {
    [Test]
    public void Visit_AdminProductsPage_ShouldSeeBottle()
    {
      driver.Navigate().GoToUrl(viteUrl);

      // Login as VIP...
      GoToLoginPage();
      LoginAsVip();

      // Go to Admin Products page...
      GoToAdminProducts();

      // Should see Bottle details
      string? expectedText = "Product ID 1 Title Drink Bottle Category 3 Price 20.00 Description Dont forget to drink water, while your out doing water sports.";
      Assert.That(bottleRowTextResult, Does.Contain(expectedText), "ShouldSeeBottle - Bottle details - incorrect.");
    }
  }
}
