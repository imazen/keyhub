using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeyHub.Model;

namespace KeyHub.Data
{
    public abstract class AbstractFilteredDataContext : DataContext
    {
        protected abstract IEnumerable<Guid> ResolveAuthorizedVendors();
        protected abstract bool ContextIsForSystemAdmin();
        protected abstract bool ContextIsForVendorAdmin();
        protected abstract IEnumerable<Guid> GetUserCustomerRights();
        protected abstract IEnumerable<Guid> GetUserLicenseRights();

        protected void ApplyFilters()
        {
            var authorizedVendorIds = ResolveAuthorizedVendors();
            this.Vendors = new FilteredDbSet<Vendor>(this, v => authorizedVendorIds.Contains(v.ObjectId));
            this.Features = new FilteredDbSet<Feature>(this, f => authorizedVendorIds.Contains(f.VendorId));

            //License dependant entities.
            var authorizedLicenseIds = ResolveAuthorizedLicenses();
            this.Licenses = new FilteredDbSet<License>(this, l => Enumerable.Contains(authorizedLicenseIds, l.ObjectId));
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this,
                lc => Enumerable.Contains(authorizedLicenseIds, lc.LicenseId));

            //SKU dependant entities.
            var authorizedSKUIds = Enumerable.Concat<Guid>(ResolveAuthorizedSKUsByAuthorizedLicenses(), ResolveAuthorizedSKUsByAuthorizedVendors()).ToList();
            this.SKUs = new FilteredDbSet<SKU>(this, s => authorizedSKUIds.Contains(s.SkuId));

            //Transaction items depends on current user role
            if (ContextIsForSystemAdmin() || ContextIsForVendorAdmin())
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => authorizedSKUIds.Contains(ti.SkuId));
            }
            else
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this,
                    ti => Enumerable.Contains(authorizedLicenseIds, (Guid) ti.LicenseId));
            }

            //Transaction Items app dependant entities
            var authorizedTransactions = ResolveAuthorizedTransactionsByAuthorizedTransactionItems();
            this.Transactions = new FilteredDbSet<Transaction>(this, t => Enumerable.Contains(authorizedTransactions, t.TransactionId));
            this.TransactionIgnoredItems = new FilteredDbSet<TransactionIgnoredItem>(this,
                t => Enumerable.Contains(authorizedTransactions, t.TransactionId));

            //Customer app dependant entities
            var authorizedCustomerApps = (from c in this.LicenseCustomerApps select c.CustomerAppId).ToList();
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));
            this.CustomerAppIssues = new FilteredDbSet<CustomerAppIssue>(this,
                c => authorizedCustomerApps.Contains(c.CustomerAppId));

            //Customer dependant entities
            var authorizedCustomerIds = Enumerable.Concat<Guid>(ResolveAuthorizedCustomersByAuthorizedLicenses(), ResolveAuthorizedCustomers()).ToList();
            this.Customers = new FilteredDbSet<Customer>(this, c => authorizedCustomerIds.Contains(c.ObjectId));
        }

        /// <summary>
        /// Resolve customer rights based on current user
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedCustomers()
        {
            if (ContextIsForSystemAdmin())
                return (from x in this.Set<Customer>() select x.ObjectId).ToList();
            else
                return GetUserCustomerRights();
        }

        /// <summary>
        /// Resolve authorized licenses based on a user.
        /// Licenses with authroized skus (from Vendor), licenses from authorized customers (from User)
        /// or authorized licenses (from user)
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedLicenses()
        {
            var authorizedSKUIds = ResolveAuthorizedSKUsByAuthorizedVendors();
            var authorizedCustomerIds = ResolveAuthorizedCustomers();

            return (from l in this.Set<License>() where Enumerable.Contains(authorizedSKUIds, l.SkuId) select l.ObjectId).ToList()
                .Union
                (from l in this.Set<License>() where authorizedCustomerIds.Contains(l.PurchasingCustomerId) select l.ObjectId).ToList()
                .Union
                (from l in this.Set<License>() where authorizedCustomerIds.Contains(l.OwningCustomerId) select l.ObjectId).ToList()
                .Union
                (GetUserLicenseRights()).ToList();
        }

        /// <summary>
        /// Resolve authorized skus based on authorized vendors
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedSKUsByAuthorizedVendors()
        {
            if (Vendors == null)
                throw new DbSetNotReadyException("Unable to resolve authorized SKUs by authorized vendors, vendor DbSet is not set!");

            var authorizedVendorIds = (from v in this.Vendors select v.ObjectId).ToList();
            return (from s in this.Set<SKU>() where authorizedVendorIds.Contains(s.VendorId) select s.SkuId).ToList();
        }

        /// <summary>
        /// Based on the current set of licenses resolve skus
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedSKUsByAuthorizedLicenses()
        {
            if (Licenses == null)
                throw new DbSetNotReadyException("Unable to resolve authorized SKUs by authorized licenses, license DbSet is not set!");

            return (from l in this.Licenses select l.SkuId).ToList();
        }

        /// <summary>
        /// Based on the current set of transaction items resolve transactions
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedTransactionsByAuthorizedTransactionItems()
        {
            if (TransactionItems == null)
                throw new DbSetNotReadyException("Unable to resolve authorized transactions by authorized transaction items, transaction items DbSet is not set!");

            return  (from t in this.TransactionItems select t.TransactionId)
                .Union(from t in this.TransactionIgnoredItems select t.TransactionId)
                .ToList();
        }

        /// <summary>
        /// Based on the current set of licenses resolve customers
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedCustomersByAuthorizedLicenses()
        {
            if (Licenses == null)
                throw new DbSetNotReadyException("Unable to resolve authorized customers by authorized licenses, license DbSet is not set!");

            return (from l in this.Licenses select l.PurchasingCustomerId).ToList()
                .Union
                (from l in this.Licenses select l.OwningCustomerId).ToList();
        }
    }
}