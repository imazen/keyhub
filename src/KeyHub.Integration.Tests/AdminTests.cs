using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class AdminTests
    {
        [Fact]
        [CleanDatabase]
        public void CanCreateVendorWithSharedSecret()
        {
            using (var site = new KeyHubWebDriver())
            {
                //using (var browser = BrowserUtil.GetBrowser())
                var browser = BrowserUtil.GetBrowser();
                {
                    browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));

                    browser.Navigate().GoToUrl(site.UrlFor("/Vendor/Create"));
                    browser.FindElementByCssSelector("input#UserName").SendKeys("admin");
                    browser.FindElementByCssSelector("input#Password").SendKeys("password");
                    browser.FindElementByCssSelector("input[type=submit]").Click();

                    browser.FindElementByCssSelector("input#Vendor_Name").SendKeys("vendor name");
                    browser.FindElementByCssSelector("input#Vendor_Street").SendKeys("vendor street");
                    browser.FindElementByCssSelector("input#Vendor_PostalCode").SendKeys("vendor street");
                    browser.FindElementByCssSelector("input#Vendor_City").SendKeys("vendor city");
                    browser.FindElementByCssSelector("input#Vendor_Region").SendKeys("vendor region");

                    browser.FindElementByCssSelector("form[action='/Vendor/Create'] input[type=submit]").Click();

                    browser.FindElementByCssSelector("a[href^='/VendorSecret/Create']").Click();

                    browser.FindElementByCssSelector("input#CredentialName").SendKeys("vendor secret name");
                    browser.FindElementByCssSelector("input#CredentialValue").SendKeys("vendor secret shared secret");

                    browser.FindElementByCssSelector("form[action^='/VendorSecret/Create'] input[type=submit]").Click();

                    browser.FindElementByCssSelector("foo.barr");
                    // TODO: Make sure edit button works

                    // TODO: Make sure the Remove button works
                }
            }
        }
    }
}
