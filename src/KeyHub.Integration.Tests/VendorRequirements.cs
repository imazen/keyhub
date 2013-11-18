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
            var vendorAndCustomerScenario = new VendorWithALicensedCustomer();

            using (var site = new KeyHubWebDriver())
            {
                vendorAndCustomerScenario.Setup(site);

                var firstCustomerAppName = "customerApp.name1";
                var secondCustomerAppName = "customerApp.name2";

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, vendorAndCustomerScenario.UserEmail, vendorAndCustomerScenario.UserPassword);
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

        [Fact]
        [CleanDatabase]
        public void VendorCanManageDomainLicenses()
        {
            using (var site = new KeyHubWebDriver())
            {
                var scenario = new VendorWithALicensedCustomer();
                scenario.Setup(site, canDeleteManualDomainsOfLicense:true);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    //  Create a license
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, scenario.UserEmail, scenario.UserPassword);

                    browser.FindElementByCssSelector("a[href='/License']").Click();
                    browser.FindElementByCssSelector("a[href^='/License/Details']").Click();

                    browser.FindElementByCssSelector("a[href^='/DomainLicense/Create']").Click();
                    browser.FindElementByCssSelector("input#DomainName").SendKeys("example.com");
                    browser.FindElementByCssSelector("form[action^='/DomainLicense/Create'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");

                    var licensedDomains = GetLicensedDomains(browser);
                    Assert.Contains("example.com", licensedDomains);

                    //  Edit a license
                    browser.FindElementByCssSelector("a[href^='/DomainLicense/Edit']").Click();
                    IWebElement domainNameInput = browser.FindElementByCssSelector("input#DomainName");
                    domainNameInput.Clear();
                    domainNameInput.SendKeys("example.org");
                    browser.FindElementByCssSelector("form[action^='/DomainLicense/Edit'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");
                    licensedDomains = GetLicensedDomains(browser);
                    Assert.Contains("example.org", licensedDomains);
                    Assert.DoesNotContain("example.com", licensedDomains);

                    //  Delete a license
                    browser.FindElementByCssSelector("a[href^='/DomainLicense/Remove']").Click();
                    browser.FindElementByCssSelector("form[action^='/DomainLicense/Remove'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");
                    
                    licensedDomains = GetLicensedDomains(browser);
                    Assert.Equal(0, licensedDomains.Count());
                }
            }
        }

        [Fact]
        [CleanDatabase]
        public void VendorCanEditAFeature()
        {
            using (var site = new KeyHubWebDriver())
            {
                var scenario = new WithAVendor();
                scenario.Setup(site);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, scenario.UserEmail, scenario.UserPassword);

                    VendorUtil.CreateFeature(browser, "first feature", scenario.VendorName);

                    var featureRow = browser.FindElement(By.XPath("//td[contains(text(),'first feature')]/ancestor::tr"));
                    featureRow.FindElement(By.CssSelector("a[href^='/Feature/Edit']")).Click();

                    var nameInput = browser.FindElementByCssSelector("#Feature_FeatureName");
                    nameInput.Clear();
                    nameInput.SendKeys("second name");

                    browser.FindElementByCssSelector("form[action^='/Feature/Edit'] input[type='submit']").Click();
                    browser.FindElementByCssSelector(".success");

                    browser.FindElement(By.XPath("//td[contains(text(),'second name')]"));
                }
            }
        }

        private static IEnumerable<string> GetLicensedDomains(RemoteWebDriver browser)
        {
            var licensedDomains = browser.FindElementsByCssSelector("a[href^='/DomainLicense/Edit']")
                .Select(a => a.FindElement(By.XPath("./ancestor::tr/td[1]")).Text.Trim())
                .ToArray();

            return licensedDomains;
        }

        public class WithAVendor
        {
            //  The vendor is the user, VendorName is the name of the Vendor object not the User.
            public string UserEmail = "vendor@example.com";
            public string UserPassword = "vendorPassword";
            public string VendorName;

            public void Setup(KeyHubWebDriver site)
            {
                //  The vendor creates their user account
                SiteUtil.CreateLocalAccount(site, UserEmail, UserPassword);

                //  The admin makes that user account a vendor.
                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    VendorName = AdminUtil.CreateVendor(browser);

                    AdminUtil.CreateAccountRightsFor(browser, UserEmail, ObjectTypes.Vendor,
                        VendorName);
                }
            }
        }

        public class VendorWithALicensedCustomer : WithAVendor
        {
            public void Setup(KeyHubWebDriver site, bool canDeleteManualDomainsOfLicense = true)
            {
                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, UserEmail, UserPassword);

                    VendorUtil.CreatePrivateKey(browser, VendorName);

                    VendorUtil.CreateFeature(browser, "first feature", VendorName);
                    VendorUtil.CreateFeature(browser, "second feature", VendorName);

                    VendorUtil.CreateSku(browser, "first sku", VendorName, "first feature", canDeleteManualDomainsOfLicense);
                    VendorUtil.CreateSku(browser, "second sku", VendorName, "second feature", canDeleteManualDomainsOfLicense);

                    //  Create a Customer
                    var customerName = VendorUtil.CreateCustomer(browser);

                    //  Create a License
                    VendorUtil.CreateLicense(browser, "first sku", customerName);
                    VendorUtil.CreateLicense(browser, "second sku", customerName);
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
