using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace KeyHub.Integration.Tests.TestSetup
{
    class VendorUtil
    {
        public static void CreatePrivateKey(RemoteWebDriver browser, string vendorName)
        {
            browser.FindElementByCssSelector("a[href='/Vendor']").Click();
            browser.FindElement(By.LinkText(vendorName)).Click();
            browser.FindElementByCssSelector("a[href^='/PrivateKey/Create']").Click();
            browser.FindElementByCssSelector("input#DisplayName").SendKeys("privatekey.Name");
            browser.FindElementByCssSelector("form[action^='/PrivateKey/Create'] input[type='submit']").Click();
        }

        public static string CreateCustomer(RemoteWebDriver browser)
        {
            var customerName = "customer.name";

            browser.FindElementByCssSelector("a[href='/Customer']").Click();
            browser.FindElementByCssSelector("a[href='/Customer/Create']").Click();

            browser.FindElementByCssSelector("#Customer_Name").SendKeys(customerName);
            browser.FindElementByCssSelector("#Customer_Department").SendKeys("customer.department");
            browser.FindElementByCssSelector("#Customer_Street").SendKeys("customer.street");
            browser.FindElementByCssSelector("#Customer_PostalCode").SendKeys("customer.postalcode");
            browser.FindElementByCssSelector("#Customer_City").SendKeys("customer.city");
            browser.FindElementByCssSelector("#Customer_Region").SendKeys("customer.region");
            browser.FindElementByCssSelector("input[value='Create Customer']").Click();

            browser.FindElementByCssSelector(".success");

            return customerName;
        }

        public static void CreateFeature(RemoteWebDriver browser, string featureName, string vendorName)
        {
            browser.FindElementByCssSelector("a[href='/Feature']").Click();
            browser.FindElementByCssSelector("a[href='/Feature/Create']").Click();
            browser.FindElementByCssSelector("input#Feature_FeatureName").SendKeys(featureName);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#Feature_VendorId_chzn", vendorName);
            browser.FindElementByCssSelector("form[action='/Feature/Create'] input[type='submit']").Click();
            browser.FindElementByCssSelector(".success");
        }

        public static void CreateSku(RemoteWebDriver browser, string skuCode, string vendorName, string featureName)
        {
            browser.FindElementByCssSelector("a[href='/SKU']").Click();
            browser.FindElementByCssSelector("a[href='/SKU/Create']").Click();
            SiteUtil.SetValueForChosenJQueryControl(browser, "#SKU_VendorId_chzn", vendorName);
            browser.FindElementByCssSelector("input#SKU_SkuCode").SendKeys(skuCode);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#SKU_SelectedFeatureGUIDs_chzn", featureName);
            browser.FindElementByCssSelector("input#SKU_LicenseDuration").SendKeys("100");
            browser.FindElementByCssSelector("input#SKU_AutoDomainDuration").SendKeys("100");
            browser.FindElementByCssSelector("form[action='/SKU/Create'] input[type='submit']").Click();
            browser.FindElementByCssSelector(".success");
        }

        public static void CreateLicense(RemoteWebDriver browser, string skuCode, string customerName)
        {
            browser.FindElementByCssSelector("a[href='/License']").Click();
            browser.FindElementByCssSelector("a[href='/License/Create']").Click();
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_SkuId_chzn", skuCode);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_PurchasingCustomerId_chzn", customerName);
            browser.FindElementByCssSelector("input#License_OwnerName").SendKeys(customerName);
            SiteUtil.SetValueForChosenJQueryControl(browser, "#License_OwningCustomerId_chzn", customerName);

            SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseIssued", DateTime.Now);
            SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseExpires", DateTime.Now + TimeSpan.FromDays(100));

            browser.FindElementByCssSelector("input[type='submit'][value='Create License']").Click();
            browser.FindElementByCssSelector(".success");
        }
    }
}
