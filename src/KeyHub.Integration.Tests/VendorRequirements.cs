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
using OpenQA.Selenium.Support.UI;
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

                    vendorName = AdminUtil.CreateVendor(browser);

                    AdminUtil.CreateAccountRightsFor(browser, userEmail, ObjectTypes.Vendor, vendorName);
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, userEmail, userPassword);

                    VendorUtil.CreatePrivateKey(browser, vendorName);

                    VendorUtil.CreateFeature(browser, "first feature", vendorName);
                    VendorUtil.CreateFeature(browser, "second feature", vendorName);

                    VendorUtil.CreateSku(browser, "first sku", vendorName, "first feature");
                    VendorUtil.CreateSku(browser, "second sku", vendorName, "second feature");

                    //  Create a Customer
                    var customerName = VendorUtil.CreateCustomer(browser);

                    //  Create a License
                    VendorUtil.CreateLicense(browser, "first sku", customerName);
                    VendorUtil.CreateLicense(browser, "second sku", customerName);

                    //  Create a CustomerApp / Licensed Application
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href='/CustomerApp']").Click();
                    browser.FindElementByCssSelector("a[href='/CustomerApp/Create']").Click();
                    browser.FindElementByCssSelector("input#ApplicationName").SendKeys(firstCustomerAppName);
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#SelectedLicenseGUIDs_chzn", "first sku");
                    browser.FindElementByCssSelector("form[action='/CustomerApp/Create'] input[type=submit]").Click();
                    browser.FindElementByCssSelector(".success");

                    AssertApplicationNameIs(browser, firstCustomerAppName);
                    AssertApplicationSkuIs(browser, "first sku");

                    //  Rename the customer app
                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Edit']").Click();
                    var nameElement = browser.FindElementByCssSelector("input#ApplicationName");
                    nameElement.Clear();
                    nameElement.SendKeys(secondCustomerAppName);
                    browser.FindElementByCssSelector("form[action^='/CustomerApp/Edit'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");

                    AssertApplicationNameIs(browser, secondCustomerAppName);

                    // Switch licenses on the customer app
                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Edit']").Click();
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#SelectedLicenseGUIDs_chzn",
                        Keys.Backspace + Keys.Backspace + "second sku");
                    browser.FindElementByCssSelector("form[action^='/CustomerApp/Edit'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");

                    AssertApplicationSkuIs(browser, "second sku");

                    // Remove the customer app
                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Remove']").Click();
                    browser.FindElementByCssSelector("form[action^='/CustomerApp/Remove'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");
                    
                    Assert.Equal(0, browser.FindElementsByCssSelector("a[href^='/CustomerApp/Remove']").Count());
                }
            }
        }

        private void AssertApplicationNameIs(RemoteWebDriver browser, string expectedName)
        {
            var applicationLink = browser.FindElementByCssSelector("a[href^='/CustomerApp/Details']");
            Assert.Equal(expectedName, applicationLink.Text.Trim());
        }

        private void AssertApplicationSkuIs(RemoteWebDriver browser, string expectedName)
        {
            var applicationLink = browser.FindElementByCssSelector("a[href^='/CustomerApp/Details']");
            var skuElement = applicationLink.FindElement(By.XPath("./ancestor::tr/td[3]"));
            Assert.Equal(expectedName, skuElement.Text.Trim());
        }
    }
}
