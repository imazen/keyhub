using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

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

        public static void SetValueForChosenJQueryControl(RemoteWebDriver browser, string cssSelector, string value)
        {
            browser.FindElementByCssSelector(cssSelector);  // ensure the form field is present

            //  Click a contained ".chzn-single" element, if available
            var clickTarget = browser.FindElementByCssSelector(cssSelector + " .chzn-single");
            clickTarget.Click();

            browser.FindElementByCssSelector(cssSelector + " input[type=text]").SendKeys(value + Keys.Tab);
        }

        public static void SetValueForChosenJQueryControlMulti(
            RemoteWebDriver browser, 
            string cssSelector, string value, 
            bool clearExisting = false)
        {
            var clickTarget = browser.FindElementByCssSelector(cssSelector);

            clickTarget.Click();

            if (clearExisting)
            {
                foreach (var removeButton in browser.FindElementsByCssSelector(cssSelector + " li .search-choice-close"))
                {
                    removeButton.Click();
                }
            }

            var selection =
                browser.FindElementsByCssSelector(cssSelector + " li").FirstOrDefault(e => e.Text.Contains(value));

            Assert.NotNull(selection);

            selection.Click();
        }

        public static void SetDateValueForJQueryDatepicker(RemoteWebDriver browser, string elementSelector, DateTime value)
        {
            var formattedDate = value.ToString("d MMMM yyyy");
            browser.FindElementByCssSelector(elementSelector);
            browser.ExecuteScript("$(arguments[0]).datepicker('setDate', arguments[1]);", elementSelector, formattedDate);
        }
    }
}
