using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using KeyHub.Data;
using KeyHub.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using KeyHub.Web.Api.Controllers;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class TransactionByIpnTest
    {
        private TestContext testContextInstance;
        private static readonly Guid VendorGuid = new Guid("ED20D311-3F63-436C-8D91-6BADEDF8D2BC");

        private static readonly string Request =
            "residence_country=GB&payer_business_name=Business+LLC&first_name=Michael&last_name=Crichton&payer_email=crichton%40ndj7.com&payer_phone=%2B44+1624+334410&payer_street=Douglas+Central+Telephone+Exchange%0ADalton+Street&payer_city=Douglas&payer_state=Isle+of+Man&payer_zip=IM1+3PQ&payer_country_code=GB&address_name=+&address_business_name=&address_phone=&address_street=&address_city=&address_state=&address_zip=&address_country_code=US&address_country=US&payment_date=04%3A31%3A49+Dec+04%2C+2012+MST&custom=&mc_currency=USD&business=billing%40imazen.io&mc_gross=249&mc_shipping=0&tax=0&txn_type=ppdirect&payment_type=Instant&invoice=wc8dzhx7k5j9v56yfo058bw0sgoocw0gwck8ow&buyer_ip=195.10.113.83&card_last_four=9271&card_type=MasterCard&mailing_list_status=true&charset=utf-8&item_name1=Resizer+3&item_number1=929356%7C%7CR3Bundle1Ent&mc_gross_1=249&quantity1=1&num_cart_items=1&txn_id=7EG58066DH656324X&payment_status=Completed&pending_reason=&item_name=Resizer+3&item_number=929356%7C%7CR3Bundle1Ent&quantity=1&option_name1=&option_selection1=&option_name2=&option_selection2=&option_name3=&option_selection3=&contact_phone=%2B44+1624+634410&handshake=ff35a320762dcec799d9c0bb9831577c&discount_codes=&from_name=Imazen&from_email=billing%40imazen.io&mailing_list_status=true&client_shipping_method_id=0&item_cart_position=1&sku=R3Bundle1Ent&expiry_hours=0&max_downloads=9&ej_txn_id=16356000";

        /// <summary>
        /// Initialize test by setting up mocking
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            //Prepare HttpContext and Principal
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", Request),
                new HttpResponse(new StringWriter())
            );
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity(""),
                new string[0]
                );

            //Mock datacontext
            var mockContext = new DataContext();
            Mock.Arrange(() => mockContext.Vendors).IgnoreInstance().ReturnsCollection(new List<Vendor> { new Vendor { ObjectId = VendorGuid, Name="TestVendor" } });
            //TODO, Mock any other data required by IpnTransaction request

            // Mock user instance to divert role manager
            var userMock = Mock.Create<User>();
            Mock.Arrange(() => userMock.IsVendorAdmin).IgnoreInstance().Returns(false);
            Mock.Arrange(() => userMock.IsSystemAdmin).IgnoreInstance().Returns(false);
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestIpn()
        {
            IDataContextFactory factory = null;//Mock factory
            new TransactionByIpnController(factory).PostTransactionByIpn(VendorGuid.ToString(), new System.Net.Http.Formatting.FormDataCollection(Request));
        }
    }
}
