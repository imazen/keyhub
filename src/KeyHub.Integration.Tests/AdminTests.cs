using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class AdminTests
    {
        [Fact]
        [CleanDatabase]
        public void CanManageVendorCredentials()
        {
            using (var site = new KeyHubWebDriver())
            {
                using (var browser = BrowserUtil.GetBrowser())
                {
                    //  Log in as pre-created admin user
                    browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    //  Create a vendor
                    SiteUtil.CreateVendor(browser);

                    //  Create a VendorCredential for the vendor
                    browser.FindElementByCssSelector("a[href^='/VendorCredential/Create']").Click();

                    var firstVendorCredentialName = "first vendor secret name";
                    var firstVendorCredentialValue = "vendor secret shared secret";

                    FillVendorCredentialForm(browser, firstVendorCredentialName, firstVendorCredentialValue);
                    browser.FindElementByCssSelector("form[action^='/VendorCredential/Create'] input[type=submit]").Click();

                    //  Make sure the VendorCredential was created, and edit it.
                    var editButton = browser.FindElementByCssSelector("a[href^='/VendorCredential/Edit']");
                    Assert.Contains(firstVendorCredentialName, browser.PageSource);
                    editButton.Click();

                    AssertVendorCredentialFormValues(browser, firstVendorCredentialName, firstVendorCredentialValue);
                    var secondVendorCredentialName = "second vendor secret name";
                    var secondVendorCredentialValue = "second vendor secret";
                    FillVendorCredentialForm(browser, secondVendorCredentialName, secondVendorCredentialValue);
                    browser.FindElementByCssSelector("form[action^='/VendorCredential/Edit'] input[type=submit]").Click();

                    //  Check the VendorCredential edit page to ensure the edit saved
                    editButton = browser.FindElementByCssSelector("a[href^='/VendorCredential/Edit']");
                    Assert.DoesNotContain(firstVendorCredentialName, browser.PageSource);
                    Assert.Contains(secondVendorCredentialValue, browser.PageSource);
                    editButton.Click();

                    AssertVendorCredentialFormValues(browser, secondVendorCredentialName, secondVendorCredentialValue);

                    //  Return to the Vendor edit page
                    browser.FindElementByCssSelector("a[href^='/Vendor/Details']").Click();

                    //  Remove the VendorCredential
                    var removeButton = browser.FindElementByCssSelector("a[href^='/VendorCredential/Remove']");
                    removeButton.Click();
                    browser.FindElementByCssSelector("form[action^='/VendorCredential/Remove'] input[type=submit]").Click();

                    browser.FindElementByCssSelector(".success");
                    Assert.DoesNotContain(firstVendorCredentialName, browser.PageSource);
                    Assert.DoesNotContain(secondVendorCredentialValue, browser.PageSource);
                }
            }
        }

        private static void FillVendorCredentialForm(RemoteWebDriver browser, string name, string value)
        {
            browser.FindElementByCssSelector("input#CredentialName").Clear();
            browser.FindElementByCssSelector("input#CredentialName").SendKeys(name);
            browser.FindElementByCssSelector("input#CredentialValue").Clear();
            browser.FindElementByCssSelector("input#CredentialValue").SendKeys(value);
        }

        private static void AssertVendorCredentialFormValues(RemoteWebDriver browser, string name, string value)
        {
            Assert.Equal(name, browser.FindElementByCssSelector("input#CredentialName").GetAttribute("value"));
            Assert.Equal(value, browser.FindElementByCssSelector("input#CredentialValue").GetAttribute("value"));
        }
    }
}
