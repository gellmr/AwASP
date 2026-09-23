using NUnitTests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;

namespace NUnitTests.SeleniumTests
{
  [TestFixture]
  public class AdminCustAccountsTest : AdminTest
  {
    const string editPageUserPicCss = ".adminUserEditCurrPhoto";
    const string editFullNameCss = "#editFullName";
    const string editUserNameCss = "#editUserName";
    const string editUserIdCss   = "#editUserId";
    const string editGuestIdCss  = "#editGuestId";
    const string editPhoneCss    = "#editPhone";
    const string editEmailCss    = "#editEmail";

    public string? fullName = null;
    public string? userName = null;
    public string? uid = null;
    public string? gid = null;
    public string? phone = null;
    public string? email = null;

    const string editAccLinkCss = ".editAccLink";

    [Test]
    public void LoginAsVip_ShouldSeeAdmin()
    {
      driver.Navigate().GoToUrl(viteUrl);
      GoToLoginPage();
      LoginAsVip();
      GoToCustomerAccounts();
      ShouldSee_Admin_CustomerAccountLine(); // Should see Administrator as first row
    }

    [Test]
    public void EditAccount_ShouldSeeAdmin()
    {
      driver.Navigate().GoToUrl(viteUrl);
      GoToLoginPage();
      LoginAsVip();
      GoToCustomerAccounts();
      GetCustomerAccountLines();
      GetAccountTypeAndId("Administrator");
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        customerAccountLineResultText = TestHelpers.TrimAndFlattenString(chosenCaRow.Text);
        string? expectedText = "(Logged in as) Administrator";
        Assert.That(customerAccountLineResultText, Does.Contain(expectedText), "Edit Admin - Administrator row not found.");
        IWebElement editBtn = chosenCaRow.FindElement(By.CssSelector(editAccLinkCss));
        IWebElement clickableButton = wait.Until(ExpectedConditions.ElementToBeClickable(editBtn));
        clickableButton.Click();
        // Wait until the Edit Account page appears
        IWebElement editPageUserPic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(editPageUserPicCss))); // The pic should take longest to load.
        GetCustomerDetail();
      }
      catch (WebDriverTimeoutException){
        Assert.Fail("Edit Admin - Timeout occurred");
      }
      // Should see Aministrator details
      Assert.That(fullName, Is.EqualTo("Administrator"),      "Edit Admin - Administrator fullName incorrect");
      Assert.That(userName, Is.EqualTo("user111"),            "Edit Admin - Administrator userName incorrect");
      if (gid != null){ Assert.That(gid, Is.EqualTo(guestId), "Edit Admin - Administrator gid incorrect"); }
      if (uid != null){ Assert.That(uid, Is.EqualTo(userId),  "Edit Admin - Administrator uid incorrect"); }
      Assert.That(phone, Is.EqualTo("04 1234 4321"),          "Edit Admin - Administrator phone incorrect");
      Assert.That(email, Is.EqualTo("user-111@gmail.com"),    "Edit Admin - Administrator email incorrect");
    }

    public void GetCustomerDetail()
    {
      fullName = GetInputVal(editFullNameCss);
      userName = GetInputVal(editUserNameCss);
      gid = (guestId != null) ? GetInputVal(editGuestIdCss) : null;
      uid = (userId != null) ? GetInputVal(editUserIdCss) : null;
      phone = GetInputVal(editPhoneCss);
      email = GetInputVal(editEmailCss);
    }

    [Test]
    public void EditAccount_ShouldSeeGuest_EileenRyan()
    {
      driver.Navigate().GoToUrl(viteUrl);
      GoToLoginPage();
      LoginAsVip();
      GoToCustomerAccounts();
      GetCustomerAccountLines();
      GetAccountTypeAndId("Eileen Ryan");
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        customerAccountLineResultText = TestHelpers.TrimAndFlattenString(chosenCaRow.Text);
        string? expectedText1 = "Full Name Eileen Ryan";
        string? expectedText2 = "Email eileen.ryan@freesport.com Account Type Guest Edit Account View Orders";
        Assert.That(customerAccountLineResultText, Does.Contain(expectedText1), "ShouldSeeGuest - EileenRyan - incorrect details.");
        Assert.That(customerAccountLineResultText, Does.Contain(expectedText2), "ShouldSeeGuest - EileenRyan - incorrect details.");
        editBtn.Click();

        // Wait until the Edit Account page appears
        IWebElement editPageUserPic = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(editPageUserPicCss))); // Longest load.
        GetCustomerDetail();
      }
      catch (WebDriverTimeoutException)
      {
        Assert.Fail("ShouldSeeGuest - EileenRyan - Timeout occurred");
      }
      // Should see Eileen Ryan
      Assert.That(fullName, Is.EqualTo("Eileen Ryan"),               "ShouldSeeGuest - EileenRyan - Administrator fullName incorrect");
      Assert.That(userName, Is.EqualTo("eileen-ryan-b42"),           "ShouldSeeGuest - EileenRyan - Administrator userName incorrect");
      Assert.That(gid,      Is.EqualTo(guestId),                     "ShouldSeeGuest - EileenRyan - Administrator gid incorrect");
      //Assert.That(phone,    Is.EqualTo("xx xxxx xxxx"),            "ShouldSeeGuest - EileenRyan - Administrator phone incorrect");
      Assert.That(email,    Is.EqualTo("eileen.ryan@freesport.com"), "ShouldSeeGuest - EileenRyan - Administrator email incorrect");
    }
  }
}
