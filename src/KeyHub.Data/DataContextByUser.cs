﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data
{
    public class DataContextByUser : DataContext, IDataContextByUser
    {
        /// <summary>
        /// Gets a datacontext based on a provided userIdentity
        /// </summary>
        /// <param name="userIdentity">Current user identity</param>
        /// <returns>
        /// A context based on the provided user idenity. If user is unknown or anonymous 
        /// a datacontext is still returned but will have empty collections
        /// </returns>
        public DataContextByUser(IIdentity userIdentity)
        {
            var currentUser = this.GetUser(userIdentity);

            //Vendor dependant entities.
            var authorizedVendorIds = ResolveAuthorizedVendorsByUser(currentUser);
            this.Vendors = new FilteredDbSet<Vendor>(this, v => authorizedVendorIds.Contains(v.ObjectId));
            this.Features = new FilteredDbSet<Feature>(this, f => authorizedVendorIds.Contains(f.VendorId));

            //License dependant entities.
            var authorizedLicenseIds = ResolveAuthorizedLicensesByUser(currentUser);
            this.Licenses = new FilteredDbSet<License>(this, l => authorizedLicenseIds.Contains(l.ObjectId));
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this, lc => authorizedLicenseIds.Contains(lc.LicenseId));

            //SKU dependant entities.
            var authorizedSKUIds = ResolveAuthorizedSKUsByAuthorizedLicenses()
                                   .Concat(ResolveAuthorizedSKUsByAuthorizedVendors()).ToList();
            this.SKUs = new FilteredDbSet<SKU>(this, s => authorizedSKUIds.Contains(s.SkuId));
            
            //Transaction items depends on current user role
            if (currentUser.IsSystemAdmin || currentUser.IsVendorAdmin)
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => authorizedSKUIds.Contains(ti.SkuId));
            }
            else
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => authorizedLicenseIds.Contains((Guid)ti.LicenseId));
            }
            
            //Transaction Items app dependant entities
            var authorizedTransactions = ResolveAuthorizedTransactionsByAuthorizedTransactionItems();
            this.Transactions = new FilteredDbSet<Transaction>(this, t => authorizedTransactions.Contains(t.TransactionId));
            this.TransactionIgnoredItems = new FilteredDbSet<TransactionIgnoredItem>(this, t => authorizedTransactions.Contains(t.TransactionId));

            //Customer app dependant entities
            var authorizedCustomerApps = (from c in this.LicenseCustomerApps select c.CustomerAppId).ToList();
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));
            this.CustomerAppIssues = new FilteredDbSet<CustomerAppIssue>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));

            //Customer dependant entities
            var authorizedCustomerIds = ResolveAuthorizedCustomersByAuthorizedLicenses()
                                    .Concat(ResolveAuthorizedCustomersByUser(currentUser)).ToList();
            this.Customers = new FilteredDbSet<Customer>(this, c => authorizedCustomerIds.Contains(c.ObjectId));
        }

        #region "Resolving user rights"
        /// <summary>
        /// Resolve vendor rights based on current user
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedVendorsByUser(User currentUser)
        {
            if (currentUser.IsSystemAdmin)
                return (from x in this.Set<Vendor>() select x.ObjectId).ToList();
            else
                return (from r in currentUser.Rights where r is UserVendorRight && r.RightId == VendorAdmin.Id select r.ObjectId).ToList();
        }

        /// <summary>
        /// Resolve customer rights based on current user
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedCustomersByUser(User currentUser)
        {
            if (currentUser.IsSystemAdmin)
                return (from x in this.Set<Customer>() select x.ObjectId).ToList();
            else
                return (from r in currentUser.Rights where r is UserCustomerRight && r.RightId == EditEntityMembers.Id select r.ObjectId).ToList();
        }

        /// <summary>
        /// Resolve authorized licenses based on a user.
        /// Licenses with authroized skus (from Vendor), licenses from authorized customers (from User)
        /// or authorized licenses (from user)
        /// </summary>
        protected IEnumerable<Guid> ResolveAuthorizedLicensesByUser(User currentUser)
        {
            var authorizedSKUIds = ResolveAuthorizedSKUsByAuthorizedVendors();
            var authorizedCustomerIds = ResolveAuthorizedCustomersByUser(currentUser);

            return (from l in this.Set<License>() where authorizedSKUIds.Contains(l.SkuId) select l.ObjectId).ToList()
                    .Union
                    (from l in this.Set<License>() where authorizedCustomerIds.Contains(l.PurchasingCustomerId) select l.ObjectId).ToList()
                    .Union
                    (from l in this.Set<License>() where authorizedCustomerIds.Contains(l.OwningCustomerId) select l.ObjectId).ToList()
                    .Union
                    (from r in currentUser.Rights where r is UserLicenseRight && r.RightId == EditLicenseInfo.Id select r.ObjectId).ToList();
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
        #endregion
    }
}
