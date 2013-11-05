using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace KeyHub.Integration.Tests.TestSetup
{
    class BrowserUtil
    {
        public static RemoteWebDriver GetBrowser()
        {
            //return new PhantomJSDriver(); // faster
            return new FirefoxDriver(); // easier debugging
        }
    }
}
