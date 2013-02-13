using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data
{
    public class DataContextByTransaction : DataContext, IDataContextByTransaction
    {
        /// <summary>
        /// Gets a datacontext based on single transaction ID
        /// </summary>
        /// <returns>Returns a single transaction access datacontext</returns>
        public DataContextByTransaction(int transactionId)
        {
            //Transactions depends on provided transactionId
            this.Transactions = new FilteredDbSet<Transaction>(this, t => t.TransactionId == transactionId);

            //Transaction items depends on provided transactionId
            this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => ti.TransactionId == transactionId);

            //Licenses depends on provided transactionItems
            this.Licenses = new FilteredDbSet<License>(this,
                l => TransactionItems.Select(t => t.LicenseId).Contains(l.ObjectId));

            this.Vendors = new FilteredDbSet<Vendor>(this);
            this.SKUs = new FilteredDbSet<SKU>(this);
            this.Features = new FilteredDbSet<Feature>(this);
            this.Customers = new FilteredDbSet<Customer>(this);
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this);
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this);
        }
    }
}
