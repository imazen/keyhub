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
    }
}
