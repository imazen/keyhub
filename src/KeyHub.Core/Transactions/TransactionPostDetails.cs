using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Core.Transactions
{
    public class TransactionPostDetails
    {
        public Guid[] PurchasedSkus { get; set; }

        public string PurchaserName { get; set; }

        public string PurchaserEmail { get; set; }
    }
}
