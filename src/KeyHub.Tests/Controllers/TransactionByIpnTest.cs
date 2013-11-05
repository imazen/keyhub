using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Tests.TestCore;
using KeyHub.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyHub.Web.Api.Controllers;
using Moq;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class TransactionByIpnTest
    {
        private static readonly Guid VendorGuid = new Guid("ED20D311-3F63-436C-8D91-6BADEDF8D2BC");
        private const string SkuName = "1197635||R3Performance";
        private const string Request = "residence_country=US&payer_business_name=SoftwareCo&first_name=John&last_name=Doe&" + 
        "payer_email=john%40example.com&payer_phone=5025551234&payer_street=401+Main+St.&payer_city=Jeffersonville&payer_state=IN" + 
        "&payer_zip=47130&payer_country_code=US&address_name=+&address_business_name=&address_phone=&address_street=&address_city=" + 
        "&address_state=&address_zip=&address_country_code=US&address_country=US&payment_date=07%3A15%3A08+Feb+22%2C+2012+MST&" + 
        "custom=&mc_currency=USD&business=billing%40imazen.io&mc_gross=2.49&mc_shipping=0&tax=0&txn_type=ppdirect&payment_type=" + 
        "Instant&invoice=553351351a33a5325325a325a2324&buyer_ip=10.10.10.10&card_last_four=1234&card_type=MasterCard&" + 
        "mailing_list_status=true&charset=utf-8&item_name1=Resizer+3.X&item_number1=1197635%7C%7CR3Performance&mc_gross_1=2.49" + 
        "&quantity1=1&num_cart_items=1&txn_id=00T198685A3853305&payment_status=Completed&pending_reason=&item_name=Resizer+3.X" + 
        "&item_number=1197635%7C%7CR3Performance&quantity=1&option_name1=&option_selection1=&option_name2=&option_selection2=" +  
        "&option_name3=&option_selection3=&contact_phone=502551234&handshake=nononono&discount_codes=For+Cart+Item+Total%3A+T315125125" + 
        "&from_name=Imazen&from_email=billing%40imazen.io&mailing_list_status=true&client_shipping_method_id=0&item_cart_position=1" + 
        "&sku=R3Performance&expiry_hours=0&max_downloads=9&ej_txn_id=1111111";

        private TestContext testContextInstance;
        private TransactionByIpnController controller;

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

        /// <summary>
        /// Initialize test by setting up mocking
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", Request),
                new HttpResponse(new StringWriter())
            );

            var mailService = new FakeMailService();
            
            var dataContextFactory = new FakeDataContextFactory();

            dataContextFactory.DataContext
                .Setup(x => x.GetUser(It.IsAny<IIdentity>()))
                .Returns(UserTestData.CreateAnonymous());

            dataContextFactory.DataContext
                              .Setup(x => x.Vendors)
                              .Returns(new FakeDbSet<Vendor>
                                  {
                                      new Vendor {ObjectId = VendorGuid, Name = "TestVendor"}
                                  });
            
            dataContextFactory.DataContext
                              .Setup(x => x.SKUs)
                              .Returns(new FakeDbSet<SKU>
                                  {
                                      new SKU {SkuId = Guid.NewGuid(), SkuCode = SkuName, VendorId = VendorGuid}
                                  });

            controller = new TransactionByIpnController(dataContextFactory, mailService);
        }

        [TestMethod]
        public void TransactionByIpnShouldNotFail()
        {
            controller.PostTransactionByIpn(
                VendorGuid.ToString(), 
                new System.Net.Http.
                    Formatting.FormDataCollection(Request));
        }
    }
}
