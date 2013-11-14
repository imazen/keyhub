using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class VendorRequirements
    {
        [Fact]
        [CleanDatabase]
        public void VendorCanManuallyCreateLicensedApplicationAndChangeItsSkus()
        {
            //  The vendor is the user, vendorName is the name of the Vendor object not hte User.
            var userEmail = "vendor@example.com";
            var userPassword = "vendorPassword";
            string vendorName;

            var firstCustomerAppName = "customerApp.name1";
            var secondCustomerAppName = "customerApp.name2";

            using (var site = new KeyHubWebDriver())
            {
                //  The vendor creates their user account
                SiteUtil.CreateLocalAccount(site, userEmail, userPassword);

                //  The admin makes that user account a vendor.
                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    vendorName = SiteUtil.CreateVendor(browser);

                    SiteUtil.CreateAccountRightsFor(browser, userEmail, ObjectTypes.Vendor, vendorName);
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, userEmail, userPassword);

                    browser.FindElementByCssSelector("a[href='/Vendor']").Click();
                    browser.FindElement(By.LinkText(vendorName)).Click();
                    browser.FindElementByCssSelector("a[href^='/PrivateKey/Create']").Click();
                    browser.FindElementByCssSelector("input#DisplayName").SendKeys("privatekey.Name");
                    browser.FindElementByCssSelector("form[action^='/PrivateKey/Create'] input[type='submit']").Click();

                    SiteUtil.CreateFeature(browser, "first feature", vendorName);
                    SiteUtil.CreateFeature(browser, "second feature", vendorName);

                    SiteUtil.CreateSku(browser, "first sku", vendorName, "first feature");
                    SiteUtil.CreateSku(browser, "second sku", vendorName, "second feature");

                    //  Create a Customer
                    var customerName = SiteUtil.CreateCustomer(browser);

                    //  Create a License
                    CreateLicense(browser, "first sku", customerName);
                    CreateLicense(browser, "second sku", customerName);

                    //  Create a CustomerApp / Licensed Application
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href='/CustomerApp']").Click();
                    browser.FindElementByCssSelector("a[href='/CustomerApp/Create']").Click();
                    browser.FindElementByCssSelector("input#ApplicationName").SendKeys(firstCustomerAppName);
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#SelectedLicenseGUIDs_chzn", "first sku");
                    browser.FindElementByCssSelector("form[action='/CustomerApp/Create'] input[type=submit]").Click();
                    browser.FindElementByCssSelector(".success");

                    AssertApplicationNameIs(browser, firstCustomerAppName);

                    //  Rename the customer app
                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Edit']").Click();
                    var nameElement = browser.FindElementByCssSelector("input#CustomerApp_ApplicationName");
                    nameElement.Clear();
                    nameElement.SendKeys(secondCustomerAppName);
                    browser.FindElementByCssSelector("form[action^='/CustomerApp/Edit'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");

                    AssertApplicationNameIs(browser, secondCustomerAppName);

                    // Remove the customer app
                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Remove']").Click();

                    Assert.Equal(0, browser.FindElementsByCssSelector("a[href^='/CustomerApp/Remove']").Count());
                }
            }
        }

        public static void CreateLicense(RemoteWebDriver browser, string skuCode, string customerName)
        {
            browser.FindElementByCssSelector("a[href='/License']").Click();
            browser.FindElementByCssSelector("a[href='/License/Create']").Click();
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_SkuId_chzn", skuCode);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_PurchasingCustomerId_chzn", customerName);
            browser.FindElementByCssSelector("input#License_OwnerName").SendKeys(customerName);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_OwningCustomerId_chzn", customerName);

            SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseIssued", DateTime.Now);
            SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseExpires", DateTime.Now + TimeSpan.FromDays(100));

            browser.FindElementByCssSelector("input[type='submit'][value='Create License']").Click();
            browser.FindElementByCssSelector(".success");
        }

        private void AssertApplicationNameIs(RemoteWebDriver browser, string expectedName)
        {
            var applicationLink = browser.FindElementByCssSelector("a[href^='/CustomerApp/Details']");
            Assert.Equal(expectedName, applicationLink.Text.Trim());
        }
    }
}
