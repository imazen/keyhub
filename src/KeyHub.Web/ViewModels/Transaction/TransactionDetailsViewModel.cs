using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Transaction
{
    /// <summary>
    /// Viewmodel for details of a single Transaction
    /// </summary>
    public class TransactionDetailsViewModel : BaseViewModel<Model.Transaction>
    {
        public TransactionDetailsViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="transaction">Transaction entity</param>
        public TransactionDetailsViewModel(Model.Transaction transaction)
            : this()
        {
            Transaction = new TransactionDetailsViewItem(transaction, transaction.TransactionItems);
        }

        /// <summary>
        /// Transaction viewmodel
        /// </summary>
        public TransactionDetailsViewItem Transaction { get; set; }

        /// <summary>
        /// Convert back to Transaction instance
        /// </summary>
        /// <param name="original">Original Transaction. If Null a new instance is created.</param>
        /// <returns>Transaction containing viewmodel data </returns>
        public override Model.Transaction ToEntity(Model.Transaction original)
        {
            return Transaction.ToEntity(original);
        }
    }

    /// <summary>
    /// TransactionViewModel extension that includes the name of the countrycode assinged country
    /// </summary>
    public class TransactionDetailsViewItem : TransactionViewModel
    {
        public TransactionDetailsViewItem() : base()
        { }

        public TransactionDetailsViewItem(Model.Transaction transaction, IEnumerable<Model.TransactionItem> transactionItems)
            : base(transaction)
        {
            PurchaserName = ((transactionItems.Count() > 0)&&(transactionItems.FirstOrDefault().License != null)) ?
                transactionItems.FirstOrDefault().License.PurchasingCustomer.Name : "None";

            SKUSummary = (from x in transactionItems select x.Sku).ToSummary(x => x.SkuCode, 99, ", ");

            StatusName = transaction.Status.GetDescription<Model.TransactionStatus>();
        }

        /// <summary>
        /// Name of the purchasing customer.
        /// </summary>
        [DisplayName("Purchased by")]
        public string PurchaserName { get; set; }

        /// <summary>
        /// SKUs bought by this transaction.
        /// </summary>
        [DisplayName("Purchased SKUs")]
        public string SKUSummary { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [DisplayName("Transaction status")]
        public string StatusName { get; set; }
    }
}