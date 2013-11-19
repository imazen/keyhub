using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using netDumbster.smtp;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class TransactionTests
    {
        [Fact]
        [CleanDatabase]
        public void CanUseALicensePurchasedViaEJunkie()
        {
            var vendorScenario = new WithAVendorDBScenario();
            var payerEmail = "payerEmail@example.com";

            var customerEmail = "customerEmail@example.com";
            var customerPassword = "customerPassword";

            var smtpServer = SimpleSmtpServer.Start(25);
            try
            {
                using (var site = new KeyHubWebDriver())
                {
                    using (var client = new WebClient())
                    {
                        var uploadVariables = new NameValueCollection();

                        uploadVariables.Add("handshake", vendorScenario.VendorCredential);
                        uploadVariables.Add("txn_id", GetRandomString());
                        uploadVariables.Add("payment_status", "Completed");
                        uploadVariables.Add("payment_date", "02:51:26 Jul 18, 2012 MST");
                        uploadVariables.Add("num_cart_items", "0");
                        uploadVariables.Add("payer_name", "payerName");
                        uploadVariables.Add("payer_email", payerEmail);

                        uploadVariables.Add("num_cart_items", "1");
                        uploadVariables.Add("item_name1", "item name");
                        uploadVariables.Add("item_number1", vendorScenario.SkuCode);
                        uploadVariables.Add("mc_gross_1", "350");
                        uploadVariables.Add("quantity1", "1");

                        client.UploadValues(site.UrlFor("Api/TransactionByIpn/" + vendorScenario.VendorId), uploadVariables);
                    }

                    var email = smtpServer.ReceivedEmail.Single();
                    Assert.Equal(payerEmail, email.ToAddresses.Single().Address);

                    var emailBody = Encoding.ASCII.GetString(Convert.FromBase64String(email.MessageParts.Single().BodyData));
                    var emailLinkMatch = Regex.Match(emailBody, @"\s(http[^\s]+)\s", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    Assert.True(emailLinkMatch.Success, "Did not find followup link in email body: " + emailBody);

                    var emailLink = emailLinkMatch.Groups[1].Value;

                    using (var browser = BrowserUtil.GetBrowser())
                    {
                        browser.Navigate().GoToUrl(emailLink);

                        browser.FindElementByCssSelector("a[href^='/Transaction/Checkout']").Click();

                        browser.FindElementByCssSelector("a[href^='/Account/Register']").Click();

                        SiteUtil.SubmitRegistrationForm(browser, customerEmail, customerPassword);
                        SiteUtil.WaitUntilUserIsLoggedIn(browser);

                        SubmitTransactionCheckoutFormWithNewCustomer(browser);

                        var appKeyValue = GetAppKeyFromTransactionCompletePage(browser);

                        LicenseValidatorTests.AssertRemoteValidationCheckPasses(
                            site, "example.com", 
                            appKeyValue, 
                            vendorScenario.FeatureCode, 
                            vendorScenario.PublicKeyXml);
                    }
                }
            }
            finally 
            {
                smtpServer.Stop();
            }
        }

        private static Guid GetAppKeyFromTransactionCompletePage(RemoteWebDriver browser)
        {
            var appKeyValue = Guid.Parse(browser.FindElementByCssSelector("#customerAppKeyValue").Text.Trim());
            return appKeyValue;
        }

        [Fact]
        [CleanDatabase]
        public void VendorCanManuallyCreateTransactionForNewCustomers()
        {
            VendorManuallyCreatesTransaction(browser => { },
                SubmitTransactionCheckoutFormWithNewCustomer);
        }

        [Fact]
        [CleanDatabase]
        public void VendorCanManuallyCreateTransactionForExistingCustomers()
        {
            string purchasingCustomer = "purchasing customer name";
            string owningCustomer = "owning customer name";

            VendorManuallyCreatesTransaction(browser =>
            {
                VendorUtil.CreateCustomer(browser, purchasingCustomer);
                VendorUtil.CreateCustomer(browser, owningCustomer);
            },
            browser =>
            {
                browser.FindElementByCssSelector("#cb_ExistingPurchasingCustomer").Click();
                SiteUtil.SetValueForChosenJQueryControl(browser, "#PurchasingCustomerId_chzn", purchasingCustomer);

                browser.FindElementByCssSelector("#cb_OwningCustomerIsPurchasingCustomerId").Click();

                browser.FindElementByCssSelector("#cb_ExistingOwningCustomer").Click();
                SiteUtil.SetValueForChosenJQueryControl(browser, "#OwningCustomerId_chzn", owningCustomer);

                browser.FindElementByCssSelector("form[action^='/Transaction/Checkout']  input[type=submit]").Click();
            });
        }

        private static void VendorManuallyCreatesTransaction(Action<RemoteWebDriver> vendorActionBeforeCreatingTransaction, Action<RemoteWebDriver> transactionSubmitHandler)
        {
            var vendorScenario = new WithAVendorDBScenario();
            var vendorEmail = "vendor@example.com";
            var vendorPassword = "vendorPassword";

            using (var site = new KeyHubWebDriver())
            {
                string editVendorUserUrl = null;

                SiteUtil.CreateLocalAccount(site, vendorEmail, vendorPassword, firstBrowser =>
                {
                    firstBrowser.FindElementByCssSelector("a[href='/Account/LogOff']");
                    firstBrowser.Navigate().GoToUrl(site.UrlFor("/Account"));

                    editVendorUserUrl =
                        firstBrowser.FindElementByCssSelector("a[href^='/Account/Edit']").GetAttribute("href");
                });

                //  Log in as admin to give the new vendor account vendor permissions
                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor(editVendorUserUrl));

                    SiteUtil.SubmitLoginForm(browser, "admin", "password");

                    AdminUtil.CreateAccountRightsFor(browser, vendorEmail, ObjectTypes.Vendor, vendorScenario.VendorName);
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    SiteUtil.SubmitLoginForm(browser, vendorEmail, vendorPassword);

                    vendorActionBeforeCreatingTransaction(browser);

                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    browser.FindElementByCssSelector("a[href='/Transaction/Create']").Click();

                    SiteUtil.SetValueForChosenJQueryControl(browser, "div#Transaction_SelectedSKUGuids_chzn",
                        vendorScenario.SkuCode);

                    browser.FindElementByCssSelector("form[action^='/Transaction/Create'] input[type='submit']").Click();

                    transactionSubmitHandler(browser);

                    var appKeyValue = GetAppKeyFromTransactionCompletePage(browser);

                    LicenseValidatorTests.AssertRemoteValidationCheckPasses(
                        site, "example.com",
                        appKeyValue,
                        vendorScenario.FeatureCode,
                        vendorScenario.PublicKeyXml);
                }
            }
        }

        private static void SubmitTransactionCheckoutFormWithNewCustomer(RemoteWebDriver browser)
        {
            var formSelector = "form[action^='/Transaction/Checkout'] ";
            browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Name")
                .SendKeys("customerName");
            browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Street")
                .SendKeys("123 Fake St.");
            browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_PostalCode").SendKeys("98105");
            browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_City").SendKeys("Seattle");
            browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Region").SendKeys("WA");
            browser.FindElementByCssSelector(formSelector + "input[type=submit]").Click();
        }

        private static string GetRandomString()
        {
            return "rndstr" + string.Join("", Guid.NewGuid().ToString().Split('-'));
        }
    }
}
