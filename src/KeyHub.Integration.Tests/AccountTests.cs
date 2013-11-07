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
                CreateLocalAccount(site, email, password);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                    browser.Navigate().GoToUrl(site.UrlFor("/Account/Login"));
                    browser.FindElementByCssSelector("input[name=provider][value=Google]").Click();

                    FillGoogleLoginForm(browser, email, password);

                    var errorText = browser.FindElementByCssSelector(".error").Text;
                    Assert.Contains("The email address used to login is already in use", errorText);

                    browser.Navigate().GoToUrl(site.UrlFor("/Account/LinkAccount"));
                    SubmitLoginForm(browser, email, password);

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
            var firstEmail = "flippant@example.com";
            var secondEmail = "flippant2@example.com";
            var password = "myPassword";

            using (var site = new KeyHubWebDriver())
            {
                CreateLocalAccount(site, firstEmail, password);

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SubmitLoginForm(browser, firstEmail, password);
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href^='/Account']").Click();
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']").Click();

                    var emailForm = browser.FindElementByCssSelector("#User_Email");
                    emailForm.Clear();
                    emailForm.SendKeys(secondEmail);
                    browser.FindElementByCssSelector("input[value='Save']").Click();

                    // Ensure the change saves by waiting for the browser to return to the account edit page
                    browser.FindElementByCssSelector("a[href^='/Account/Edit']");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SubmitLoginForm(browser, secondEmail, password);
                    WaitUntilUserIsLoggedIn(browser);
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
                CreateLocalAccount(site, email, password);
                CreateLocalAccount(site, email, password, browser =>
                {
                    var errorText = browser.FindElementByCssSelector(".validation-summary-errors li").Text;
                    Assert.Contains("The email address registered is already in use", errorText);
                });
            }
        }

        public static void CreateLocalAccount(KeyHubWebDriver site, string email, string password, Action<RemoteWebDriver> onFinish = null)
        {
            onFinish = onFinish ?? delegate(RemoteWebDriver browser)
            {
                WaitUntilUserIsLoggedIn(browser);
            };

            using (var browser = BrowserUtil.GetBrowser())
            {
                browser.Navigate().GoToUrl(site.UrlFor("Account/Register"));

                SubmitRegistrationForm(browser, email, password);

                onFinish(browser);
            }
        }

        private static IWebElement WaitUntilUserIsLoggedIn(RemoteWebDriver browser)
        {
            return browser.FindElementByCssSelector("a[href='/Account/LogOff']");
        }

        public static void SubmitLoginForm(RemoteWebDriver browser, string email, string password)
        {
            var formSelector = "form[action^='/Account/Login'] ";
            browser.FindElementByCssSelector(formSelector + "input#UserName").SendKeys(email);
            browser.FindElementByCssSelector(formSelector + "input#Password").SendKeys(password);
            browser.FindElementByCssSelector(formSelector + "input[value='Log in']").Click();
        }

        public static void SubmitRegistrationForm(RemoteWebDriver browser, string email, string password)
        {
            browser.FindElementByCssSelector("input[name=Email]").SendKeys(email);
            browser.FindElementByCssSelector("input[name=Password]").SendKeys(password);
            browser.FindElementByCssSelector("input[name=ConfirmPassword]").SendKeys(password);
            browser.FindElementByCssSelector("input[value=Register]").Click();
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
