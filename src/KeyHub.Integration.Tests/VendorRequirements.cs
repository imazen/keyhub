using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class VendorRequirements
    {
        [Fact]
        [CleanDatabase]
        public void VendorCanManuallyCreateLicensedApplicationAndChangeItsSkus()
        {
            var vendorScenario = new HasVendorScenario();

            var vendorEmail = "vendor@example.com";
            var vendorPassword = "vendorPassword";

            using (var site = new KeyHubWebDriver())
            {
                //  Create a user account for the vendor, with vendor rights
                SiteUtil.CreateLocalAccount(site, vendorEmail, vendorPassword);

                using (var context = new DataContext())
                {
                    var user = context.Users.Single(u => u.Email == vendorEmail);
                    user.Rights.Add(new UserVendorRight()
                    {
                        ObjectId = vendorScenario.VendorId,
                        RightId = Model.VendorAdmin.Id,
                    });

                    context.SaveChanges();
                }

                {
                    var browser = BrowserUtil.GetBrowser();

                    //  Create a Customer
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, vendorEmail, vendorPassword);
                    var customerName = SiteUtil.CreateCustomer(browser);

                    //  Create a License
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href='/License']").Click();
                    browser.FindElementByCssSelector("a[href='/License/Create']").Click();
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#License_SkuId_chzn", vendorScenario.SkuCode);
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#License_PurchasingCustomerId_chzn", customerName);
                    browser.FindElementByCssSelector("input#License_OwnerName").SendKeys("original owner name");
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#License_OwningCustomerId_chzn", customerName);

                    SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseIssued", DateTime.Now);
                    SiteUtil.SetDateValueForJQueryDatepicker(browser, "input#License_LicenseExpires", DateTime.Now + TimeSpan.FromDays(100));

                    browser.FindElementByCssSelector("input[type='submit'][value='Create License']").Click();
                    browser.FindElementByCssSelector(".success");

                    //  Create a CustomerApp / Licensed Application
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href='/CustomerApp']").Click();
                    browser.FindElementByCssSelector("a[href='/CustomerApp/Create']").Click();

                    browser.FindElementByCssSelector("input#CustomerApp_ApplicationName").SendKeys("customerApp.name");
                    SiteUtil.SetValueForChosenJQueryControl(browser, "#CustomerApp_SelectedLicenseGUIDs_chzn", vendorScenario.SkuCode);

                    browser.FindElementByCssSelector("form[action='/CustomerApp/Create'] input[type=submit]").Click();

                    browser.FindElementByCssSelector(".success");

                    browser.FindElementByCssSelector("a[href^='/CustomerApp/Edit']").Click();

                    //  Rename the CustomerCap

                    //  browser.FindElementByCssSelector("foo");

                    //  Remove the sku
                }
            }


        }
    }
}
