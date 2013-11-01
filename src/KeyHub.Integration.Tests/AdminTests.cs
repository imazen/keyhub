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
        public void CanManageVendorSecret()
        {
            using (var site = new KeyHubWebDriver())
            {
                using (var browser = BrowserUtil.GetBrowser())
                {
                    //  Log in as pre-created admin user
                    browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                    browser.Navigate().GoToUrl(site.UrlFor("/Vendor/Create"));
                    browser.FindElementByCssSelector("input#UserName").SendKeys("admin");
                    browser.FindElementByCssSelector("input#Password").SendKeys("password");
                    browser.FindElementByCssSelector("input[type=submit]").Click();

                    //  Create a vendor
                    FillAndSubmitCreateVendorForm(browser);

                    //  Create a VendorSecret for the vendor
                    browser.FindElementByCssSelector("a[href^='/VendorSecret/Create']").Click();

                    var firstVendorSecretName = "first vendor secret name";
                    var firstVendorSecret = "vendor secret shared secret";

                    FillVendorSecretForm(browser, firstVendorSecretName, firstVendorSecret);
                    browser.FindElementByCssSelector("form[action^='/VendorSecret/Create'] input[type=submit]").Click();

                    //  Make sure the VendorSecret was created, and edit it.
                    var editButton = browser.FindElementByCssSelector("a[href^='/VendorSecret/Edit']");
                    Assert.Contains(firstVendorSecretName, browser.PageSource);
                    editButton.Click();

                    AssertVendorSecretFormValues(browser, firstVendorSecretName, firstVendorSecret);
                    var secondVendorSecretName = "second vendor secret name";
                    var secondVendorSecret = "second vendor secret";
                    FillVendorSecretForm(browser, secondVendorSecretName, secondVendorSecret);
                    browser.FindElementByCssSelector("form[action^='/VendorSecret/Edit'] input[type=submit]").Click();

                    //  Check the VendorSecret edit page to ensure the edit saved
                    editButton = browser.FindElementByCssSelector("a[href^='/VendorSecret/Edit']");
                    Assert.DoesNotContain(firstVendorSecretName, browser.PageSource);
                    Assert.Contains(secondVendorSecret, browser.PageSource);
                    editButton.Click();

                    AssertVendorSecretFormValues(browser, secondVendorSecretName, secondVendorSecret);

                    //  Return to the Vendor edit page
                    browser.FindElementByCssSelector("a[href^='/Vendor/Details']").Click();

                    //  Remove the VendorSecret
                    var removeButton = browser.FindElementByCssSelector("a[href^='/VendorSecret/Remove']");
                    removeButton.Click();
                    browser.FindElementByCssSelector("form[action^='/VendorSecret/Remove'] input[type=submit]").Click();

                    browser.FindElementByCssSelector(".success");
                    Assert.DoesNotContain(firstVendorSecretName, browser.PageSource);
                    Assert.DoesNotContain(secondVendorSecret, browser.PageSource);
                }
            }
        }

        private static void FillVendorSecretForm(RemoteWebDriver browser, string vendorSecretName, string vendorSecretSharedSecret)
        {
            browser.FindElementByCssSelector("input#CredentialName").Clear();
            browser.FindElementByCssSelector("input#CredentialName").SendKeys(vendorSecretName);
            browser.FindElementByCssSelector("input#CredentialValue").Clear();
            browser.FindElementByCssSelector("input#CredentialValue").SendKeys(vendorSecretSharedSecret);
        }

        private static void AssertVendorSecretFormValues(RemoteWebDriver browser, string vendorSecretName, string vendorSecretSharedSecret)
        {
            Assert.Equal(vendorSecretName, browser.FindElementByCssSelector("input#CredentialName").GetAttribute("value"));
            Assert.Equal(vendorSecretSharedSecret, browser.FindElementByCssSelector("input#CredentialValue").GetAttribute("value"));
        }

        private static void FillAndSubmitCreateVendorForm(RemoteWebDriver browser)
        {
            browser.FindElementByCssSelector("input#Vendor_Name").SendKeys("vendor name");
            browser.FindElementByCssSelector("input#Vendor_Street").SendKeys("vendor street");
            browser.FindElementByCssSelector("input#Vendor_PostalCode").SendKeys("vendor street");
            browser.FindElementByCssSelector("input#Vendor_City").SendKeys("vendor city");
            browser.FindElementByCssSelector("input#Vendor_Region").SendKeys("vendor region");

            browser.FindElementByCssSelector("form[action='/Vendor/Create'] input[type=submit]").Click();
        }
    }
}
