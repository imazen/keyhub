using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.Transaction
{
    /// <summary>
    /// Viewmodel for index list of a Transaction
    /// </summary>
    public class TransactionIndexViewModel : BaseViewModel<Model.Transaction>
    {
        public TransactionIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="transactionList">List of transaction entities</param>
        public TransactionIndexViewModel(List<Model.Transaction> transactionList):this()
        {
            Transactions = new List<TransactionIndexViewItem>();
            foreach (Model.Transaction entity in transactionList)
            {
                Transactions.Add(new TransactionIndexViewItem(entity, entity.TransactionItems));
            }
        }

        /// <summary>
        /// List of transactions
        /// </summary>
        public List<TransactionIndexViewItem> Transactions { get; set; }

        /// <summary>
        /// Convert back to Transaction instance
        /// </summary>
        /// <param name="original">Original Transaction. If Null a new instance is created.</param>
        /// <returns>Transaction containing viewmodel data </returns>
        public override Model.Transaction ToEntity(Model.Transaction original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// TransactionViewModel extension that includes the name of the countrycode assinged country
    /// </summary>
    public class TransactionIndexViewItem : TransactionViewModel
    {
        public TransactionIndexViewItem(Model.Transaction transaction, IEnumerable<Model.TransactionItem> transactionItems)
            : base(transaction)
        {
            if (transactionItems.FirstOrDefault().License != null)
            {
                PurchaserName = transactionItems.FirstOrDefault().License.PurchasingCustomer.Name;
            }
            else
            {
                PurchaserName = "None";
            }

            SKUSummary = BuildSKUSummary((from x in transactionItems select x.Sku).ToList());
        }

        /// <summary>
        /// Builds a summary of SKUs assigned to the transaction
        /// </summary>
        /// <param name="skus">Purchased SKUs for this transaction</param>
        /// <returns>Summary</returns>
        private string BuildSKUSummary(IEnumerable<Model.SKU> skus)
        {
            const int MAXLINES = 3;
            const string LINEFEED = ", ";
            string summary = "";

            var filteredSkus = skus.Take(MAXLINES);

            if (filteredSkus.Count() > 0)
            {
                summary = string.Join(LINEFEED, filteredSkus.Select(x => x.SkuCode));
                if (skus.Count() > MAXLINES)
                    summary += string.Format(" and {0} more...", skus.Count() - MAXLINES);
            }
            else
                summary = "None";

            return summary;
        }

        /// <summary>
        /// Countryname of the customer.
        /// </summary>
        [DisplayName("Purchased by")]
        public string PurchaserName { get; set; }

        /// <summary>
        /// SKU summary.
        /// </summary>
        [DisplayName("SKUs")]
        public string SKUSummary { get; set; }
    }
}