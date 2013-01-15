using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace KeyHub.Model
{
    public partial class Transaction
    {
        /// <summary>
        /// Add SKUs to Transaction based on selected SKU Guids
        /// </summary>
        /// <param name="SKUGuids">List of selected SKU Guids</param>
        public void AddTransactionItems(IEnumerable<Guid> SKUGuids)
        {
            foreach (Guid skuGuid in SKUGuids)
            {
                if (!(from x in this.TransactionItems where x.SkuId == skuGuid select x).Any())
                    this.TransactionItems.Add(new TransactionItem() { TransactionId = this.TransactionId, SkuId = skuGuid });
            }
        }

        /// <summary>
        /// Check if transaction is waitong for claim
        /// </summary>
        public bool IsWaitingForClaim
        {
            get { return Status == TransactionStatus.Create || Status == TransactionStatus.Remind; }
        }

        /// <summary>
        /// Purchaser name from original request
        /// </summary>
        public string PurchaserName
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(OriginalRequest);

                XmlElement root = doc.DocumentElement;

                return root != null ? root.Attributes["PurchaserName"].Value : string.Empty;
            }
        }

        /// <summary>
        /// Purchaser email from original request
        /// </summary>
        public string PurchaserEmail
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(OriginalRequest);

                XmlElement root = doc.DocumentElement;

                return root != null ? root.Attributes["PurchaserEmail"].Value : string.Empty;
            }
        }
    }
}
