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
        /// Adds a tranction item to this transaction
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddTransactionItem(TransactionItem item)
        {
            if (TransactionItems.All(x => x.SkuId != item.SkuId))
                TransactionItems.Add(item);
        }

        /// <summary>
        /// Adds an ignored item to this transaction
        /// </summary>
        /// <param name="ignoredItem">The ignored item to add</param>
        public void AddIgnoredItem(TransactionIgnoredItem ignoredItem)
        {
            if (IgnoredItems.All(x => x.Description != ignoredItem.Description))
                IgnoredItems.Add(ignoredItem);
        }
        
        /// <summary>
        /// Check if transaction is waitong for claim
        /// </summary>
        public bool IsWaitingForClaim
        {
            get { return Status == TransactionStatus.Create || Status == TransactionStatus.Remind; }
        }
    }
}
