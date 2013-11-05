using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using netDumbster.smtp;
using Newtonsoft.Json;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class TransactionTests
    {
        [Fact]
        [CleanDatabase]
        public void CanRecordATransaction()
        {
            var vendorScenario = new HasVendorScenario();

            var smtpServer = SimpleSmtpServer.Start(25);
            try
            {
                using (var site = new KeyHubWebDriver())
                using (var client = new WebClient())
                {
                    var uploadVariables = new NameValueCollection();

                    uploadVariables.Add("handshake", vendorScenario.VendorCredential);
                    uploadVariables.Add("txn_id", GetRandomString());
                    uploadVariables.Add("payment_status", "Completed");
                    uploadVariables.Add("payment_date", "02:51:26 Jul 18, 2012 MST");
                    uploadVariables.Add("num_cart_items", "0");
                    uploadVariables.Add("payer_name", "payerName");
                    uploadVariables.Add("payer_email", "payerEmail@example.com");

                    uploadVariables.Add("num_cart_items", "1");
                    uploadVariables.Add("item_name1", "item name");
                    uploadVariables.Add("item_number1", vendorScenario.SkuCode);
                    uploadVariables.Add("mc_gross_1", "350");
                    uploadVariables.Add("quantity1", "1");

                    client.UploadValues(site.UrlFor("Api/TransactionByIpn/" + vendorScenario.VendorId), uploadVariables);
                }

                foreach (var email in smtpServer.ReceivedEmail)
                {
                    var summary = new
                    {
                        To = String.Join(",", email.ToAddresses.Select(t => t.ToString())),
                        From = email.FromAddress.ToString(),
                        Subject = email.Headers["Subject"]
                    };
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
