using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpnTransactionPlugin.Model
{
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
}
