using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Xml;
using IpnTransactionPlugin.Extensions;
using IpnTransactionPlugin.Model;
using KeyHub.Core.Transactions;

namespace IpnTransactionPlugin
{
    [Export("ejunkie")]
    public class IpnTransactionHandler : ITransactionPostHandler
    {
        // POST api/ipn/vendor
        public TransactionPostDetails InterpretTransactionPost(HttpRequestMessage data)
        {
            var d = (dynamic)data;

            //To calculate 'handshake', run 'md5 -s [password]', then 'md5 -s email@domain.com[Last MD5 result]'
            if (!"ff35a320762dcec799d9c0bb9831577c".Equals(d.Pluck("handshake", null), StringComparison.OrdinalIgnoreCase)) throw new Exception("Invalid handshake provided");

            string txn_id = d.Pluck("txn_id");
            //TODO: We must ignore duplicate POSTs with the same txn_id - all POSTs will contain the same information

            if (!"Completed".Equals(d.Pluck("payment_status"), StringComparison.OrdinalIgnoreCase)) throw new Exception("Only completed transactions should be sent to this URL");

            var txn = new Transaction();
            txn.VendorId = "";
            txn.ExternalTransactionId = txn_id;
            txn.PaymentDate = d.Pluck<DateTime>("payment_date").Value;
            txn.PayerEmail = d.Pluck("payer_email");
            txn.PassThroughData = d.Pluck("custom");
            txn.Currency = d.Pluck("mc_currency");
            txn.InvoiceId = d.Pluck("invoice");
            txn.Gross = d.Pluck<double>("mc_gross");
            txn.TransactionFee = d.Pluck<double>("mc_fee");
            txn.Tax = d.Pluck<double>("tax");
            txn.TransactionType = d.Pluck("transaction_type");

            if (d["payer_name"] == null) d["payer_name"] = d["first_name"] + " " + d["last_name"];

            txn.Billing = NameAndAddress.ParseFrom(d, "payer_");
            txn.Shipping = NameAndAddress.ParseFrom(d, "address_");

            var itemCount = d.Pluck<int>("num_cart_items", 1);

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

                var qty = d.Pluck<int>("quantity" + i.ToString(), 1).Value;
                for (var j = 0; j < qty; j++)
                    txn.Items.Add(item.DeepCopy());
            }

            if (!string.IsNullOrEmpty(d["gc_xml"]))
            {
                txn.ProcessorXml = new XmlDocument();
                txn.ProcessorXml.LoadXml(d.Pluck("gc_xml"));
            }

            txn.Other = null; //postedData;

            return txn.ToTransactionDetailsClass();
        }

        
    }
}
