using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace KeyHub.Integration.Tests.TestSetup
{
    class AdminUtil
    {
        public static string CreateVendor(RemoteWebDriver browser)
        {
            string vendorName = "vendor name";

            browser.FindElementByCssSelector("a[href='/Vendor']").Click();
            browser.FindElementByCssSelector("a[href='/Vendor/Create']").Click();

            browser.FindElementByCssSelector("input#Vendor_Name").SendKeys(vendorName);
            browser.FindElementByCssSelector("input#Vendor_Street").SendKeys("vendor street");
            browser.FindElementByCssSelector("input#Vendor_PostalCode").SendKeys("vendor street");
            browser.FindElementByCssSelector("input#Vendor_City").SendKeys("vendor city");
            browser.FindElementByCssSelector("input#Vendor_Region").SendKeys("vendor region");

            browser.FindElementByCssSelector("form[action='/Vendor/Create'] input[type=submit]").Click();
            browser.FindElementByCssSelector(".success");

            return vendorName;
        }

        public static void CreateAccountRightsFor(RemoteWebDriver browser, string userEmail, ObjectTypes objectType, string objectName)
        {
            browser.FindElementByCssSelector("a[href='/Account']").Click();
            var vendorUserRow =
                browser.FindElementByLinkText(userEmail).FindElement(By.XPath("./ancestor::tr"));

            vendorUserRow.FindElement(By.CssSelector("a[href^='/Account/Edit']")).Click();

            browser.FindElementByCssSelector("a[href^='/AccountRights/Create'][href$='" + Enum.GetName(typeof(ObjectTypes), objectType) + "']").Click();

            SiteUtil.SetValueForChosenJQueryControl(browser, "#ObjectId_chzn", objectName);
            browser.FindElementByCssSelector("form[action^='/AccountRights/Create'] input[type='submit']").Click();
            browser.FindElementByCssSelector(".success");
        }
    }
}
