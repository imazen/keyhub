using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            var domainlessLicenseQuery =
                (from x in transaction.TransactionItems
                 where (x.License != null) && (!x.License.Domains.Any())
                 select x.License);

            var customerapplessLicenseQuery =
                (from x in transaction.TransactionItems
                 where (x.License != null) && (!x.License.LicenseCustomerApps.Any())
                 select x.License);

            Transaction = new TransactionDetailsViewItem(transaction, 
                transaction.TransactionItems, 
                transaction.IgnoredItems,
                domainlessLicenseQuery.ToList(),
                customerapplessLicenseQuery.ToList());
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

        public TransactionDetailsViewItem(Model.Transaction transaction, 
            IEnumerable<Model.TransactionItem> transactionItems,
            IEnumerable<Model.TransactionIgnoredItem> transactionIgnoredItems,
            IEnumerable<Model.License> domainlessLicenseQuery,
            IEnumerable<Model.License> customerapplessLicenseQuery)
            : base(transaction)
        {
            PurchaserEmail = transaction.PurchaserEmail;

            PurchaserName = (transactionItems.Any(l => l.License != null)) ?
                transactionItems.FirstOrDefault().License.PurchasingCustomer.Name : transaction.PurchaserName;

            OwnerName = (transactionItems.Any(l => l.License != null)) ?
                transactionItems.FirstOrDefault().License.OwningCustomer.Name : "None";

            SKUSummary = (from x in transactionItems select x.Sku).ToSummary(x => x.SkuCode, 99, ", ");

            IgnoredSummary = (from x in transactionIgnoredItems select x.Description).ToSummary(x => x, 99, ", ");

            StatusName = transaction.Status.GetDescription<Model.TransactionStatus>();

            DomainlessLicenses = (from x in domainlessLicenseQuery select x.ObjectId).ToList();

            CustomerapplessLicenses = (from x in customerapplessLicenseQuery select x.ObjectId).ToList();
        }

        /// <summary>
        /// Email of the purchaser.
        /// </summary>
        [DisplayName("Purchaser Email")]
        [DataType(DataType.EmailAddress)]
        public string PurchaserEmail { get; set; }

        /// <summary>
        /// Name of the purchasing customer.
        /// </summary>
        [DisplayName("Purchased by")]
        public string PurchaserName { get; set; }

        /// <summary>
        /// Name of the owning customer.
        /// </summary>
        [DisplayName("Owned by")]
        public string OwnerName { get; set; }

        /// <summary>
        /// SKUs bought by this transaction.
        /// </summary>
        [DisplayName("Purchased SKUs")]
        public string SKUSummary { get; set; }

        /// <summary>
        /// SKUs ignored for this transation.
        /// </summary>
        [DisplayName("Ignored SKUs")]
        public string IgnoredSummary { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [DisplayName("Transaction status")]
        public string StatusName { get; set; }

        /// <summary>
        /// List of Licenses within transaction that do not have a domain.
        /// Used to show create DomainLicense partial view instead of License index partial view
        /// </summary>
        public IEnumerable<Guid> DomainlessLicenses { get; set; }

        /// <summary>
        /// List of Licenses within transaction that do not have a customer app.
        /// Used to show create CustomerApp partial view instead of CusomterApp index partial view
        /// </summary>
        public IEnumerable<Guid> CustomerapplessLicenses { get; set; }
    }
}