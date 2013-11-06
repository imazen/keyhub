﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
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
            var vendorScenario = new HasVendorScenario();
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
                        browser.FindElementByCssSelector("a[href^='/Account/Register']").Click();

                        AccountTests.SubmitRegistrationForm(browser, customerEmail, customerPassword);

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
        public void VendorCanManuallyCreateOrder()
        {
            var vendorScenario = new HasVendorScenario();
            var vendorEmail = "vendorEmail@example.com";
            var vendorPassword = "vendorPassword";

            using (var site = new KeyHubWebDriver())
            {
                string editVendorUserUrl = null;

                AccountTests.CreateLocalAccount(site, vendorEmail, vendorPassword, firstBrowser =>
                {
                    firstBrowser.FindElementByCssSelector("a[href='/Account/LogOff']");
                    firstBrowser.Navigate().GoToUrl(site.UrlFor("/Account"));

                    editVendorUserUrl =
                        firstBrowser.FindElementByCssSelector("a[href^='/Account/Edit']").GetAttribute("href");
                });

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor(editVendorUserUrl));

                    AccountTests.SubmitLoginForm(browser, "admin", "password");

                    browser.FindElementByCssSelector("a[href^='/AccountRights/Create']").Click();

                    browser.FindElementByCssSelector("#ObjectId_chzn").Click();
                    browser.Keyboard.SendKeys(vendorScenario.VendorName + Keys.Enter);

                    browser.FindElementByCssSelector("input[type='submit'][value='Create']").Click();

                    //  To ensure the vendor right was created, we check that the delete button renders.
                    browser.FindElementByCssSelector("a[href^='/AccountRights/Delete']");
                }

                using (var browser = BrowserUtil.GetBrowser())
                {
                    browser.Navigate().GoToUrl(site.UrlFor("/"));
                    AccountTests.SubmitLoginForm(browser, vendorEmail, vendorPassword);
                    browser.FindElementByCssSelector("a[href='/Transaction/Create']").Click();

                    browser.FindElementByCssSelector("div#Transaction_SelectedSKUGuids_chzn").Click();
                    browser.Keyboard.SendKeys(vendorScenario.SkuCode + Keys.Enter);

                    browser.FindElementByCssSelector("form[action^='/Transaction/Create'] input[type='submit']").Click();

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