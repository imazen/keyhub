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
        [Description("Transaction ready for checkout")] 
        Create = 0,
        [Description("Transaction ready for checkout. Email resent.")]
        Remind = 1,
        [Description("Licenses available")] 
        Complete = 99
    }
}
