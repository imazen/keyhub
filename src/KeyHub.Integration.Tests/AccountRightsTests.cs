using System;
using System.Collections.Generic;
using System.Linq;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class AccountRightsTests
    {
        [Fact]
        [CleanDatabase]
        public void AdminShouldBeAbleToAddAndRemoveUserRights()
        {
            using (var site = new KeyHubWebDriver())
            {
                string userEmail = "username@example.com";

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href^='/Account/Register']").Click();
                    SiteUtil.SubmitRegistrationForm(browser, userEmail, "password");
                    SiteUtil.WaitUntilUserIsLoggedIn(browser);

                    browser.FindElementByCssSelector("a[href^='/Account']").Click();
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']").GetAttribute("href");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    var customerName = SiteUtil.CreateCustomer(browser);

                    SiteUtil.CreateAccountRightsFor(browser, userEmail, ObjectTypes.Customer, customerName);

                    browser.FindElementByCssSelector(".account-rights-table");
                    var accountRights = browser.FindElementsByCssSelector(".account-rights-table tbody tr");
                    
                    Assert.Equal(1, accountRights.Count());
                    Assert.Contains(customerName, accountRights.First().Text);

                    accountRights.First().FindElement(By.CssSelector("a[href^='/AccountRights/Delete']")).Click();

                    browser.FindElementByCssSelector("input[value='Confirm Delete']").Click();

                    new WebDriverWait(browser, TimeSpan.FromSeconds(2)).Until(waitBrowser =>
                    {
                        return browser.FindElementByCssSelector(".account-rights-table") != null
                               && browser.FindElementsByCssSelector(".account-rights-table tbody tr").Count() == 0;
                    });
                }
            }
        }
    }
}
