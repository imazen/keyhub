using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class SiteUtil
    {
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

        public static IWebElement WaitUntilUserIsLoggedIn(RemoteWebDriver browser)
        {
            return browser.FindElementByCssSelector("a[href='/Account/LogOff']");
        }

        public static void SubmitLoginForm(RemoteWebDriver browser, string email, string password)
        {
            var formSelector = "form[action^='/Account/Login'] ";
            browser.FindElementByCssSelector(formSelector + "input#Email").SendKeys(email);
            browser.FindElementByCssSelector(formSelector + "input#Password").SendKeys(password);
            browser.FindElementByCssSelector(formSelector + "input[value='Log in']").Click();
            WaitUntilUserIsLoggedIn(browser);
        }

        public static void SubmitRegistrationForm(RemoteWebDriver browser, string email, string password)
        {
            browser.FindElementByCssSelector("input[name=Email]").SendKeys(email);
            browser.FindElementByCssSelector("input[name=Password]").SendKeys(password);
            browser.FindElementByCssSelector("input[name=ConfirmPassword]").SendKeys(password);
            browser.FindElementByCssSelector("input[value=Register]").Click();
        }
    }
}
