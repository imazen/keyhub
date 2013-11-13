using System;
using System.Collections.Generic;
using System.Linq;
using KeyHub.Integration.Tests.TestSetup;
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
                string userEditUrl;

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href^='/Account/Register']").Click();
                    SiteUtil.SubmitRegistrationForm(browser, "username@example.com", "password");
                    SiteUtil.WaitUntilUserIsLoggedIn(browser);

                    browser.FindElementByCssSelector("a[href^='/Account']").Click();
                    userEditUrl = browser.FindElementByCssSelector("a[href^='/Account/Edit']").GetAttribute("href");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    var customerName = SiteUtil.CreateCustomer(browser);

                    browser.Navigate().GoToUrl(userEditUrl);
                    browser.FindElementByCssSelector("a[href^='/AccountRights/Create'][href*='Customer']").Click();

                    SiteUtil.SetValueForChosenJQueryControl(browser, "#ObjectId_chzn", customerName);
                    browser.FindElementByCssSelector("input[value='Create']").Click();

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
