using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using OpenQA.Selenium;
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

                    browser.FindElementByCssSelector("a[href='/Customer']").Click();
                    browser.FindElementByCssSelector("a[href='/Customer/Create']").Click();

                    browser.FindElementByCssSelector("#Customer_Name").SendKeys("customer.name");
                    browser.FindElementByCssSelector("#Customer_Department").SendKeys("customer.department");
                    browser.FindElementByCssSelector("#Customer_Street").SendKeys("customer.street");
                    browser.FindElementByCssSelector("#Customer_PostalCode").SendKeys("customer.postalcode");
                    browser.FindElementByCssSelector("#Customer_City").SendKeys("customer.city");
                    browser.FindElementByCssSelector("#Customer_Region").SendKeys("customer.region");
                    browser.FindElementByCssSelector("input[value='Create Customer']").Click();

                    browser.FindElementByCssSelector(".success");

                    browser.Navigate().GoToUrl(userEditUrl);
                    browser.FindElementByCssSelector("a[href^='/AccountRights/Create'][href*='Customer']").Click();

                    SiteUtil.SetValueForChosenJQueryControl(browser, "#ObjectId_chzn", "customer.name");
                    browser.FindElementByCssSelector("input[value='Create']").Click();

                    browser.FindElementByCssSelector(".account-rights-table");
                    var accountRights = browser.FindElementsByCssSelector(".account-rights-table tbody tr");
                    
                    Assert.Equal(1, accountRights.Count());
                    Assert.Contains("customer.name", accountRights.First().Text);

                    accountRights.First().FindElement(By.CssSelector("a[href^='/AccountRights/Delete']")).Click();

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
