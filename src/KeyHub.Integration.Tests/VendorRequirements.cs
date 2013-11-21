using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
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
            var vendorAndCustomerScenario = new VendorWithALicensedCustomerScenario();

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
                    SiteUtil.SetValueForChosenJQueryControlMulti(browser, "#SelectedLicenseGUIDs_chzn", "first sku");
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
                    SiteUtil.SetValueForChosenJQueryControlMulti(browser, "#SelectedLicenseGUIDs_chzn", "second sku", clearExisting:true);
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
                var scenario = new VendorWithALicensedCustomerScenario();
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
                var scenario = new WithAVendorScenario();
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

        [Fact]
        [CleanDatabase]
        public void VendorCanRenameAndRemovePrivateKeys()
        {
            using (var site = new KeyHubWebDriver())
            {
                var scenario = new WithAVendorScenario();
                scenario.Setup(site);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, scenario.UserEmail, scenario.UserPassword);

                    var privateKeyName = VendorUtil.CreatePrivateKey(browser, scenario.VendorName);

                    var privateKeyRow = browser.FindElementByXPath("//td[contains(text(),'" + privateKeyName + "')]/ancestor::tr");
                    privateKeyRow.FindElement((By.CssSelector("a[href^='/PrivateKey/Edit']"))).Click();

                    var nameInput = browser.FindElementByCssSelector("input#PrivateKey_DisplayName");
                    nameInput.Clear();
                    nameInput.SendKeys("second name");
                    browser.FindElementByCssSelector("form[action^='/PrivateKey/Edit'] input[type='submit']").Click();

                    privateKeyRow = browser.FindElementByXPath("//td[contains(text(),'second name')]/ancestor::tr");

                    Assert.Equal(1, browser.FindElementsByCssSelector(".private-key-list").Count());
                    Assert.Equal(1, browser.FindElementsByCssSelector(".private-key-list tbody tr").Count());
                    privateKeyRow.FindElement((By.CssSelector("a[href^='/PrivateKey/Remove']"))).Click();

                    browser.FindElementByCssSelector("form[action^='/PrivateKey/Remove'] input[type='submit']").Click();

                    Assert.Equal(1, browser.FindElementsByCssSelector(".private-key-list").Count());
                    Assert.Equal(0, browser.FindElementsByCssSelector(".private-key-list tbody tr").Count());
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
