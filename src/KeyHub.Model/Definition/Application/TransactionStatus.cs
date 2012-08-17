using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Statuses of a transaction
    /// </summary>
    public enum TransactionStatus
    {
        [Description("SKUs selected, waiting for customer details")] 
        Create = 0,
        [Description("Checkout complete, waiting for payment")] 
        CheckoutComplete,
        [Description("Purchase pending, waiting for payment to start")]
        PurchaseStart,
        [Description("Payment pending, waiting for payment to complete")] 
        PurchasePending,
        [Description("Purchase complete, licenses available")] 
        Complete
    }
}
