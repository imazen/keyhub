using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
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

                var browser = new FirefoxDriver();
                browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                browser.Navigate().GoToUrl(site.UrlFor("/Account/Login"));
                browser.FindElementByCssSelector("input[name=provider][value=Google]").Click();

                FillGoogleLoginForm(browser, email, password);

                var errorText = browser.FindElementByCssSelector(".validation-summary-errors li").Text;
                Assert.Contains("The email address used to login is already in use", errorText);

                browser.Navigate().GoToUrl(site.UrlFor("/Account/LinkLogin"));
                browser.FindElementByCssSelector("input#UserName").SendKeys(email);
                browser.FindElementByCssSelector("input#Password").SendKeys(password);
                browser.FindElementByCssSelector("input[value='Log in']").Click();

                // TODO: verify returnUrl was honored  (need to start auth flow on an authenticated page,
                // check that we're there now)

                browser.Navigate().GoToUrl(site.UrlFor("/Account/LinkLogin"));
                Console.WriteLine("Page is " + browser.Url);
                browser.FindElementByCssSelector("input[name=provider][value=Google]").Click();

                var successText = browser.FindElementByCssSelector(".flash-info").Text;
                Assert.Contains("Your google login has been linked", successText);
            }
        }

        private static void CreateLocalAccount(KeyHubWebDriver site, string email, string password)
        {
            using (var browser = new PhantomJSDriver())
            {
                browser.Navigate().GoToUrl(site.UrlFor("Account/Register"));

                browser.FindElementByCssSelector("input[name=Email]").SendKeys(email);
                browser.FindElementByCssSelector("input[name=Password]").SendKeys(password);
                browser.FindElementByCssSelector("input[name=ConfirmPassword]").SendKeys(password);
                browser.FindElementByCssSelector("input[value=Register]").Click();

                browser.FindElementByCssSelector("a[href='/Account/LogOff']");
            }
        }

        private static void FillGoogleLoginForm(FirefoxDriver browser, string email, string password)
        {
            browser.FindElementByCssSelector("form#gaia_loginform");
            browser.FindElementByCssSelector("input#Email").SendKeys(email);
            browser.FindElementByCssSelector("input#Passwd").SendKeys(password);
            browser.FindElementByCssSelector("input#signIn").Click();
        }
    }
}
