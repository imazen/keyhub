using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace KeyHub.Integration.Tests.TestSetup
{
    class BrowserUtil
    {
        public static RemoteWebDriver GetBrowser()
        {
            //return new PhantomJSDriver(); // faster
            var result = new FirefoxDriver(); // easier debugging
            result.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            return result;
        }

        public static void SetValueForChosenJQueryControl(RemoteWebDriver browser, string cssSelector, string value)
        {
            // We're using the jQuery Chosen library for some front-end widgets.  These require special 
            // handling to get the timing correct.

            var selector = browser.FindElementByCssSelector(cssSelector);

            selector.Click();
            
            var wait = new WebDriverWait(browser, TimeSpan.FromSeconds(2));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssSelector + " input[type=text]")));

            browser.FindElementByCssSelector(cssSelector + " input[type=text]").SendKeys(value + Keys.Enter);
        }
    }
}
