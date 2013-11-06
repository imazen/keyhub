using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using netDumbster.smtp;
using Newtonsoft.Json;
using OpenQA.Selenium;
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

            var customerName = "customerName";
            var customerEmail = "customerEmail@example.com";
            var customerPassword = "customerPassword";
            var customerStreet = "123 Fake St.";
            var customerPostalCode = "98105";
            var customerCity = "Seattle";
            var customerRegion = "WA";

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

                        AccountTests.SubmitRegisterForm(browser, customerEmail, customerPassword);

                        var formSelector = "form[action^='/Transaction/Checkout'] ";
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Name").SendKeys(customerName);
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Street").SendKeys(customerStreet);
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Street").SendKeys(customerStreet);
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_PostalCode").SendKeys(customerPostalCode);
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_City").SendKeys(customerCity);
                        browser.FindElementByCssSelector(formSelector + "input#NewPurchasingCustomer_Customer_Region").SendKeys(customerRegion);
                        browser.FindElementByCssSelector(formSelector + "input[type=submit]").Click();

                        var appKeyValue = Guid.Parse(browser.FindElementByCssSelector("#customerAppKeyValue").Text.Trim());

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

        private static string GetRandomString()
        {
            return "rndstr" + string.Join("", Guid.NewGuid().ToString().Split('-'));
        }
    }
}
