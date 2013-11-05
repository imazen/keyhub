using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using KeyHub.Model;

namespace KeyHub.Data
{
    public class DataContextByTransaction : DataContextByUser, IDataContextByTransaction
    {
        /// <summary>
        /// Gets a datacontext based on single transaction ID
        /// </summary>
        /// <returns>Returns a single transaction access datacontext</returns>
        public DataContextByTransaction(IIdentity userIdentity, Guid transactionId)
            : base(userIdentity)
        {
            var currentUser = GetUser(userIdentity);

            //Vendor dependant entities.
            var authorizedVendorIds = ResolveAuthorizedVendorsByUser(currentUser);
            Vendors = new FilteredDbSet<Vendor>(this, v => authorizedVendorIds.Contains(v.ObjectId));
            Features = new FilteredDbSet<Feature>(this, f => authorizedVendorIds.Contains(f.VendorId));

            //License dependant entities.
            var authorizedLicenseIds = ResolveAuthorizedLicensesByUser(currentUser)
                                       .Concat(ResolveAuthorizedLicensesByTransactionId(transactionId)).ToList();
            Licenses = new FilteredDbSet<License>(this, l => authorizedLicenseIds.Contains(l.ObjectId));
            LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this, lc => authorizedLicenseIds.Contains(lc.LicenseId));

            //SKU dependant entities.
            var authorizedSkuIds = ResolveAuthorizedSKUsByAuthorizedLicenses()
                                   .Concat(ResolveAuthorizedSKUsByAuthorizedVendors()).ToList();
            SKUs = new FilteredDbSet<SKU>(this, s => authorizedSkuIds.Contains(s.SkuId));

            //Transaction items depends on current user role
            if (currentUser.IsVendorAdmin)
            {
                TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => authorizedSkuIds.Contains(ti.SkuId));
            }
            else
            {
                //Depends on licenses and provided transaction
                var transactionItemIdsByTransaction = ResolveAuthorizedTransactionItemsByTransactionId(transactionId);
                TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => ((authorizedLicenseIds.Contains((Guid)ti.LicenseId)) || (transactionItemIdsByTransaction.Contains(ti.TransactionItemId))));
            }

            var transactionIgnoredItemIdsByTransaction = ResolveAuthorizedTransactionIgnoredItemsByTransactionId(transactionId);
            TransactionIgnoredItems = new FilteredDbSet<TransactionIgnoredItem>(this, ti => transactionIgnoredItemIdsByTransaction.Contains(ti.TransactionItemId));

            //Transaction Items app dependant entities
            var authorizedTransactions = ResolveAuthorizedTransactionsByAuthorizedTransactionItems();
            Transactions = new FilteredDbSet<Transaction>(this, t => authorizedTransactions.Contains(t.TransactionId));

            //Customer app dependant entities
            var authorizedCustomerApps = (from c in LicenseCustomerApps select c.CustomerAppId).ToList();
            CustomerApps = new FilteredDbSet<CustomerApp>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));
            CustomerAppIssues = new FilteredDbSet<CustomerAppIssue>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));

            //Customer dependant entities
            var authorizedCustomerIds = ResolveAuthorizedCustomersByAuthorizedLicenses()
                                    .Concat(ResolveAuthorizedCustomersByUser(currentUser)).ToList();
            Customers = new FilteredDbSet<Customer>(this, c => authorizedCustomerIds.Contains(c.ObjectId));
        }

        /// <summary>
        /// Resolve authorized licenses by transaction ID
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedLicensesByTransactionId(Guid transactionId)
        {
            return (from x in Set<TransactionItem>() where x.TransactionId == transactionId && x.LicenseId.HasValue select x.LicenseId.Value).ToList();
        }

        /// <summary>
        /// Resolve authorized transactionItems by transaction ID
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedTransactionItemsByTransactionId(Guid transactionId)
        {
            return (from x in Set<TransactionItem>() where x.TransactionId == transactionId select x.TransactionItemId).ToList();
        }

        /// <summary>
        /// Resolve authorized transactionItems by transaction ID
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedTransactionIgnoredItemsByTransactionId(Guid transactionId)
        {
            return (from x in Set<TransactionIgnoredItem>() where x.TransactionId == transactionId select x.TransactionItemId).ToList();
        }
    }
}
