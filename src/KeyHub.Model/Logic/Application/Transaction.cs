using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if ((from x in this.TransactionItems where x.SkuId == skuGuid select x).Count() == 0)
                    this.TransactionItems.Add(new TransactionItem() { TransactionId = this.TransactionId, SkuId = skuGuid });
            }
        }


    }
}
