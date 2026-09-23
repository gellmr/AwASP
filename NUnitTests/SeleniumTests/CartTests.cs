using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using NUnitTests.Helpers;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class CartTests : ShopTest
  {
    public IWebElement? inCartItemOneTitle = null;

    public const string inCartItemOneCssTitle = ".inCartProd .inCartItemText h6";
    public const string inCartItemOneSuccessText = "Drink Bottle $20";
    public const string inCartRows = ".inCartProd";

    [Test]
    public void EmptyCartPage_LoadsSuccessfully()
    {
      driver.Navigate().GoToUrl(viteUrl + "/cart");
      string titleElement = ".shopLayoutTransparent h2";
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(titleElement))); // Wait for element to be visible on page.
      }
      catch (WebDriverTimeoutException){
        Assert.Fail(pageOrElementMissing);
      }
      IWebElement element = driver.FindElement(By.CssSelector(titleElement));
      Assert.That(element.Text, Does.Contain("Cart is Empty"), "Cart page - title is incorrect.");
    }

    [Test]
    public void AddToCart_IncrementsQty()
    {
      driver.Navigate().GoToUrl(viteUrl);
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
      try { AddBottleToCart(); }
      catch (WebDriverTimeoutException) { Assert.Fail(pageOrElementMissing); }
      // Here we only test for the updated text appearing in medBtn. TextToBePresentInElement fails if not visible on screen.
      try{
        wait.Until(ExpectedConditions.TextToBePresentInElement(medCartBtn,   cartHasOneItem));
        Assert.That(medCartBtn.Text,   Does.Contain(cartHasOneItem), "Cart (medBtn) - updated text is incorrect.");
      }
      catch (WebDriverTimeoutException){
        Assert.Fail("Cart (smallBtn/medBtn) - \"Cart: 1 Items\" text did not appear");
      }
      Assert.That(medCartBtn.Text,   Does.Contain(cartHasOneItem), "Cart (medBtn) - does not say 1 item.");
    }

    private void ClickAddThenGoToCart()
    {
      driver.Navigate().GoToUrl(viteUrl);
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));
      try {
        wait.Until(d => 
        {
          try {
            var layoutElement = d.FindElement(By.CssSelector("div[data-guest-ready]"));
            return layoutElement.GetAttribute("data-guest-ready") == "true";
          }
          catch (NoSuchElementException) {
            return false;
          }
        });
        AddBottleToCart();
      }
      catch (WebDriverTimeoutException) { Assert.Fail(pageOrElementMissing); }
      try
      {
        wait.Until(ExpectedConditions.TextToBePresentInElement(medCartBtn, cartHasOneItem));
        IWebElement clickableButton = wait.Until(ExpectedConditions.ElementToBeClickable(medCartBtn));
        clickableButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(inCartItemOneCssTitle)));
        inCartItemOneTitle = driver.FindElement(By.CssSelector(inCartItemOneCssTitle));
      }
      catch (WebDriverTimeoutException ex)
      {
        Assert.Fail("(ClickAddThenGoToCart) WebDriverTimeoutException: " + ex.Message);
      }
    }

    [Test]
    public void AddToCart_ItemAppearsCartPage()
    {
      ClickAddThenGoToCart();
      if (inCartItemOneTitle == null){
        Assert.Fail("Cart Page - Item One - not found"); return;
      }
      Assert.That(inCartItemOneTitle.Text, Does.Contain(inCartItemOneSuccessText), "Cart Page - Item One - text incorrect.");
    }

    

    [Test]
    public void IncrementInCart_QtyAndPriceUpdates()
    {
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));
      ClickAddThenGoToCart();
      if (inCartItemOneTitle == null){ Assert.Fail("Cart Page - Item One - not found"); return; }
      IWebElement? summaryRow = null;
      IWebElement? row1 = null;
      try
      {
        IReadOnlyCollection<IWebElement> cartRows = driver.FindElements(By.CssSelector(inCartRows));
        List<IWebElement> rows = cartRows.ToList();
        row1 = rows[0];
        summaryRow = rows[rows.Count - 1];
        IWebElement addBtn = row1.FindElement(By.CssSelector(".inCartItemRemove i.bi-plus"));
        IWebElement clickableButton = wait.Until(ExpectedConditions.ElementToBeClickable(addBtn));
        clickableButton.Click(); // Add a second item
        wait.Until(ExpectedConditions.TextToBePresentInElement(medCartBtn, cartHasTwoItem));
      }
      catch (WebDriverTimeoutException){
        Assert.Fail("Failed within Cart during IncrementInCart_QtyAndPriceUpdates test.");
      }
      if (row1 == null) {       Assert.Fail("Cart (row1)       not found"); return; }
      if (summaryRow == null) { Assert.Fail("Cart (summaryRow) not found"); return; }
      string? row1Text    = TestHelpers.TrimAndFlattenString(row1.Text);
      string? summaryText = TestHelpers.TrimAndFlattenString(summaryRow.Text);
      string priceTot = (2 * inCartItemOneUnitPrice).ToString("G29");
      Assert.That(medCartBtn.Text,     Does.Contain(cartHasTwoItem),                    "Cart (medBtn)     - Updated text is incorrect.");
      Assert.That(row1Text,        Does.Contain("Quantity: 2 Price: $" + priceTot), "Cart (row1)       - Qty text is incorrect.");
      Assert.That(summaryText,     Does.Contain("Total: 2 Items $ " + priceTot),    "Cart (summaryRow) - Qty text is incorrect.");
      Assert.That(summaryRow.Text, Does.Contain("$ " + priceTot),                   "Cart (summaryRow) - Price text is incorrect.");
    }
  }
}
