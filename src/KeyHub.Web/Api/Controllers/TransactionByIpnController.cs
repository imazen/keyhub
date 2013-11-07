using System.Data.Entity;
using System.Text;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using KeyHub.Common.Extensions;
using KeyHub.Common.Utils;
using KeyHub.Core.Mail;
using KeyHub.Data;
using KeyHub.Web.Api.Controllers.LicenseValidation;
using KeyHub.Web.Api.Controllers.Transaction;
using System.Net.Http.Formatting;
using System.Globalization;
using KeyHub.Web.Controllers;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// Ipn specific REST controller for creating new transactions. (E-JUNKIE)
    /// </summary>
    public class TransactionByIpnController : BaseTransactionController
    {
        private IDataContextFactory dataContextFactory;

        public TransactionByIpnController(IDataContextFactory dataContextFactory, IMailService mailService)
            : base(mailService)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="postedData"></param>
        public HttpResponseMessage PostTransactionByIpn([FromUri]string id, [FromBody]FormDataCollection postedData)
        {
            var vendorId = Guid.Parse(id);
            var txn = new Transaction();
            var d = postedData.ReadAsNameValueCollection();

            //To calculate 'handshake', run 'md5 -s [password]', then 'md5 -s email@domain.com[Last MD5 result]'
            string handshakeParameter = d.Pluck("handshake",null);
            if (handshakeParameter == null)
                throw new Exception("Missing parameter 'handshake'.");

            using (var dataContext = dataContextFactory.Create())
            {
                var vendor =
                    dataContext.Vendors.Where(v => v.ObjectId == vendorId)
                        .Include(v => v.VendorCredentials)
                        .FirstOrDefault();

                if (vendor == null)
                    throw new Exception("Could not find vendor with id: " + vendorId);

                string[] vendorCredentials = vendor.VendorCredentials.Select(
                    c => Encoding.UTF8.GetString(SymmetricEncryption.DecryptForDatabase(c.CredentialValue)).ToLower()).ToArray();

                if (!vendorCredentials.Contains(handshakeParameter.ToLower()))
                    throw new Exception("Invalid handshake provided");
            }

            string txn_id = d.Pluck("txn_id");
            //TODO: We must ignore duplicate POSTs with the same txn_id - all POSTs will contain the same information

            if (!"Completed".Equals(d.Pluck("payment_status"), StringComparison.OrdinalIgnoreCase)) throw new Exception("Only completed transactions should be sent to this URL");

            //var txn = new Transaction();
            txn.VendorId = vendorId;
            txn.ExternalTransactionId = txn_id;
            txn.PaymentDate = ConvertPayPalDateTime(d.Pluck("payment_date"));
            txn.PayerEmail = d.Pluck("payer_email");
            txn.PassThroughData = d.Pluck("custom");
            txn.Currency = d.Pluck("mc_currency");
            txn.InvoiceId  = d.Pluck("invoice");
            txn.Gross = d.Pluck<double>("mc_gross");
            txn.TransactionFee = d.Pluck<double>("mc_fee");
            txn.Tax = d.Pluck<double>("tax");
            txn.TransactionType = d.Pluck("transaction_type");

            if (d["payer_name"] == null) d["payer_name"] = d["first_name"] + " " + d["last_name"];

            txn.Billing = ParseFrom(d,"payer_");
            txn.Shipping = ParseFrom(d,"address_");

            var itemCount = d.Pluck<int>("num_cart_items",1);

            txn.Items = new List<TransactionItem>();
            for (var i = 1; i <= itemCount; i++)
            {
                string suffix = i.ToString();
                var item = new TransactionItem();
                item.Gross = d.Pluck<double>("mc_gross_" + i.ToString()).Value;
                item.ItemName = d.Pluck("item_name" + i.ToString());
                item.SkuString = d.Pluck("item_number" + i.ToString());
                item.Options = new List<KeyValuePair<string, string>>();
                for (var j = 1; j < 4; j++)
                {
                    string opt_key = d.Pluck("option_name" + j.ToString() + "_" + i.ToString());
                    string opt_val = d.Pluck("option_selection" + j.ToString() + "_" + i.ToString());
                    if (string.IsNullOrEmpty(opt_val)) continue;
                    item.Options.Add(new KeyValuePair<string, string>(opt_key, opt_val));
                }

                var qty = d.Pluck<int>("quantity" + i.ToString(),1).Value;
                for(var j = 0; j < qty; j++)
                    txn.Items.Add(item.DeepCopy());
            }

            if (!string.IsNullOrEmpty(d["gc_xml"])){
                txn.ProcessorXml = new XmlDocument();
                txn.ProcessorXml.LoadXml(d.Pluck("gc_xml"));
            }

            txn.Other = d;

            //All transactions go through TransactionController
            using (var basket = BasketWrapper.CreateNewByVendor(dataContextFactory, vendorId))
            {
                base.ProcessTransaction(txn.ToTransactionRequest(dataContextFactory), basket);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public static DateTime ConvertPayPalDateTime(string payPalDateTime)
        {
            payPalDateTime = KeyHub.Common.Utils.TimeZoneAbbreviations.ReplaceTimeZoneAbbreviation(payPalDateTime);

            string[] dateFormats = { "HH:mm:ss MMM dd, yyyy zzz", "HH:mm:ss MMM. dd, yyyy zzz" };

            // Parse the date. Throw an exception if it fails.
            return  DateTime.ParseExact(payPalDateTime, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private NameAndAddress ParseFrom(NameValueCollection d, string prefix)
        {
            var p = prefix;
            var n = new NameAndAddress();
            n.BusinessName = d.Pluck(p + "business_name");
            n.Name = d.Pluck(p + "name");
            n.Street = d.Pluck(p + "street");
            n.Phone = d.Pluck(p + "phone");
            n.City = d.Pluck(p + "city");
            n.Zip = d.Pluck(p + "zip");
            n.State = d.Pluck(p + "state");
            n.Country = d.Pluck(p + "country");
            n.CountryCode = d.Pluck(p + "country_code");
            return n;
        }


        public class TransactionItem
        {
            /// <summary>
            /// The generic product name (doesn't include SKU or option data)
            /// </summary>
            public string ItemName { get; set; }
            /// <summary>
            /// Used to lookup SKU - Example: "929356||R3Bundle3Pro"
            /// </summary>
            public string SkuString { get; set; }
            /// <summary>
            /// The gross cost of the item - currency specified by parent Transaction
            /// </summary>
            public double Gross { get; set; }
            /// <summary>
            /// Product customizations
            /// </summary>
            public List<KeyValuePair<string, string>> Options { get; set; }

            public TransactionItem DeepCopy()
            {
                var c = new TransactionItem();
                c.ItemName = ItemName;
                c.SkuString = SkuString;
                c.Gross = Gross;
                c.Options = new List<KeyValuePair<string, string>>();
                foreach (var p in Options)
                    c.Options.Add(new KeyValuePair<string, string>(p.Key, p.Value));
                return c;
            }
        }

        public class Transaction
        {
            /// <summary>
            /// The vendor ID for the transaction
            /// </summary>
            public Guid VendorId { get; set; }
            /// <summary>
            /// The currency used for all monetary values in this transaction
            /// </summary>
            public string Currency { get; set; }
            /// <summary>
            /// Full amount of customer payment, prior to transaction fees
            /// </summary>
            public double? Gross { get; set; }
            /// <summary>
            /// Tax applied to order
            /// </summary>
            public double? Tax { get; set; }
            /// <summary>
            /// Transaction fee associated with the paymenbt.
            /// </summary>
            public double? TransactionFee { get; set; }

            /// <summary>
            /// May be the paypal billing e-mail, google checkout e-mail, or masked google checkout e-mail
            /// </summary>
            public string PayerEmail { get; set; }

            /// <summary>
            /// Transaction ID generated by payment processor (for non-PayPal txns we add a prefix: gc- for google checkout, au- for authorize.net, 2co- for 2checkout, cb- for clickbank and tp- for trialpay payments).
            /// </summary>
            public string ExternalTransactionId { get; set; }

            /// <summary>
            /// Paypal transaction type or alternate payment provider code, such as 'cart', 'web_accept','expresscheckout', 'ppdirect', 'gc_cart','authnet','cb', '2co_cart','tp','ejgift', etc
            /// </summary>
            public string TransactionType { get; set; }

            /// <summary>
            /// Pass-through invoice ID
            /// </summary>
            public string InvoiceId { get; set; }

            /// <summary>
            /// Discount codes applied to the order
            /// </summary>
            public List<string> DiscountCodes { get; set; }

            /// <summary>
            /// Original pass-through data ('custom') associated with the transaction - can be used to implement logged-in checkout 
            /// </summary>
            public string PassThroughData { get; set; }

            public List<TransactionItem> Items { get; set; }

            public DateTime PaymentDate { get; set; }

            public NameAndAddress Billing { get; set; }

            public NameAndAddress Shipping { get; set; }

            /// <summary>
            /// Xml provided by the payment processor
            /// </summary>
            public XmlDocument ProcessorXml { get; set; }

            /// <summary>
            /// Other data associated with the transaction
            /// </summary>
            public NameValueCollection Other { get; set; }

            /// <summary>
            /// Convert transaction item to a transaction request
            /// </summary>
            /// <returns>TransactionRequest</returns>
            public TransactionRequest ToTransactionRequest(IDataContextFactory dataContextFactory)
            {
                using (var context = dataContextFactory.Create())
                {
                    var skus = new List<string>();
                    foreach (var item in Items)
                    {
                        if (!context.Vendors.Any(x => x.ObjectId == VendorId)) 
                            throw new ArgumentException(String.Format("Vendor with GUID '{0}' not found!", VendorId));

                        var sku = (from x in context.SKUs
                                   where
                                       x.VendorId == VendorId &&
                                       (x.SkuCode == item.SkuString || x.SkuAternativeCode == item.SkuString)
                                   select x).FirstOrDefault();

                        if (sku != null)
                            skus.Add(sku.SkuId.ToString());
                        else
                            skus.Add(string.Format("{0} - {1}", item.ItemName, item.SkuString));
                    }

                    return new TransactionRequest()
                    {
                        PurchasedSkus = skus.ToArray(),
                        PurchaserName = (Billing != null) ? Billing.Name : "",
                        PurchaserEmail = PayerEmail,
                    };
                }
            }
        }

        /// <summary>
        /// Contains name and address information; used for Transaction.Billing and Transaction.Shipping
        /// </summary>
        public class NameAndAddress
        {
            public string Name { get; set; }
            public string BusinessName { get; set; }
            public string Phone { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }

        }
    }
}
