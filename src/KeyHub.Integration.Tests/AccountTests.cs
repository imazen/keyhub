using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class AccountTests
    {
        [Fact]
        [CleanDatabase]
        public void CanRegisterLocallyThenAssociate3rdPartyLogin()
        {
            var email = ConfigurationManager.AppSettings.Get("googleTestEmail");
            var password = ConfigurationManager.AppSettings.Get("googleTestPassword");

            using (var site = new KeyHubWebDriver())
            {
                SiteUtil.CreateLocalAccount(site, email, password);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                    browser.Navigate().GoToUrl(site.UrlFor("/Account/Login"));
                    browser.FindElementByCssSelector("input[name=provider][value=Google]").Click();

                    FillGoogleLoginForm(browser, email, password);

                    var errorText = browser.FindElementByCssSelector(".error").Text;
                    Assert.Contains("The email address used to login is already in use", errorText);

                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, email, password);

                    browser.FindElementByCssSelector("a[href='/Account']").Click();
                    browser.FindElementByCssSelector("a[href='/Account/LinkAccount']").Click();

                    // TODO: verify returnUrl was honored  (need to start auth flow on an authenticated page,
                    // check that we're there now)

                    browser.Navigate().GoToUrl(site.UrlFor("/Account/LinkAccount"));
                    Console.WriteLine("Page is " + browser.Url);
                    browser.FindElementByCssSelector("input[name=provider][value=Google]").Click();

                    var successText = browser.FindElementByCssSelector(".success").Text;
                    Assert.Contains("Your google login has been linked", successText);
                }
            }
        }

        [Fact]
        [CleanDatabase]
        public void CanLoginLocallyAfterChangingEmail()
        {
            var firstEmail = "indecisive@example.com";
            var secondEmail = "undecisive@example.com";
            var password = "myPassword";

            using (var site = new KeyHubWebDriver())
            {
                SiteUtil.CreateLocalAccount(site, firstEmail, password);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, firstEmail, password);

                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href^='/Account']").Click();
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']").Click();

                    var emailForm = browser.FindElementByCssSelector("#Email");
                    emailForm.Clear();
                    emailForm.SendKeys(secondEmail);
                    browser.FindElementByCssSelector("input[value='Save']").Click();

                    // Ensure the change saves by waiting for the browser to return to the account edit page
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, secondEmail.ToUpper(), password);
                }
            }
        }

        [Fact]
        [CleanDatabase]
        public void CanLoginLocallyAfterChangingPassword()
        {
            var email = "indecisive@example.com";
            var firstPassword = "myPassword";
            var secondPassword = "secondPassword";

            using (var site = new KeyHubWebDriver())
            {
                SiteUtil.CreateLocalAccount(site, email, firstPassword);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, email, firstPassword);
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href^='/Account']").Click();
                    browser.FindElementByCssSelector("a[href^='/Account/ChangePassword']").Click();

                    browser.FindElementByCssSelector("#OldPassword").SendKeys(firstPassword);
                    browser.FindElementByCssSelector("#NewPassword").SendKeys(secondPassword);
                    browser.FindElementByCssSelector("#ConfirmPassword").SendKeys(secondPassword);
                    browser.FindElementByCssSelector("input[type='submit']").Click();
                    
                    // Ensure the change saves by waiting for the browser to return to the account edit page
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, email, secondPassword);
                }
            }
        }

        [Fact]
        [CleanDatabase]
        public void ShouldGiveWarningMessageWhenUserRegistersEmailAlreadyInUse()
        {
            var email = ConfigurationManager.AppSettings.Get("googleTestEmail");
            var password = ConfigurationManager.AppSettings.Get("googleTestPassword");

            using (var site = new KeyHubWebDriver())
            {
                SiteUtil.CreateLocalAccount(site, email, password);
                SiteUtil.CreateLocalAccount(site, email, password, browser =>
                {
                    var errorText = browser.FindElementByCssSelector(".validation-summary-errors li").Text;
                    Assert.Contains("The email address registered is already in use", errorText);
                });
            }
        }

        [Fact]
        [CleanDatabase]
        public void AdminShouldBeAbleToCreateUsers()
        {
            var username = "someUser@example.com";
            var password = "somePassword";

            using (var site = new KeyHubWebDriver())
            {
                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/Account/Create"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    browser.FindElementByCssSelector("#User_Email").SendKeys(username);
                    browser.FindElementByCssSelector("#User_Password").SendKeys(password);
                    browser.FindElementByCssSelector("#User_ConfirmPassword").SendKeys(password);
                    browser.FindElementByCssSelector("input[value='Save']").Click();
                    var successMessage = browser.FindElementByCssSelector(".success");
                    Assert.Contains("New user succesfully created", successMessage.Text);
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, "admin", "password");
                }
            }
        }

        private static void FillGoogleLoginForm(RemoteWebDriver browser, string email, string password)
        {
            browser.FindElementByCssSelector("form#gaia_loginform");
            browser.FindElementByCssSelector("input#Email").SendKeys(email);
            browser.FindElementByCssSelector("input#Passwd").SendKeys(password);
            browser.FindElementByCssSelector("input#signIn").Click();
        }
    }
}
