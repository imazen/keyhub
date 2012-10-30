using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Security.Principal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Runtime;
using System.Web;

namespace KeyHub.Data
{
    /// <summary>
    /// Provides data access to all tables and collections
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Gets a datacontext based on a provided userIdentity
        /// </summary>
        /// <param name="userIdentity">Current user identity</param>
        /// <returns>
        /// A context based on the provided user idenity. If user is unknown or anonymous 
        /// a datacontext is still returned but will have empty collections
        /// </returns>
        public DataContext(IIdentity userIdentity)
        {
            var currentUser = this.GetUserByIdentity(userIdentity);

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
                                   .Union(ResolveAuthorizedSKUsByAuthorizedVendors());
            this.SKUs = new FilteredDbSet<SKU>(this, s => authorizedSKUIds.Contains(s.SkuId));
            
            //Transaction items depends on current user role
            if (currentUser.IsVendorAdmin)
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => authorizedSKUIds.Contains(ti.SkuId));
            }
            else
            {
                this.TransactionItems = new FilteredDbSet<TransactionItem>(this, ti => ti.LicenseId != null && authorizedLicenseIds.Contains((Guid)ti.LicenseId));
            }

            //Transaction Items app dependant entities
            var authorizedTransactions = ResolveAuthorizedTransactionsByAuthorizedTransactionItems();
            this.Transactions = new FilteredDbSet<Transaction>(this, t => authorizedTransactions.Contains(t.TransactionId));

            //Customer app dependant entities
            var authorizedCustomerApps = (from c in this.LicenseCustomerApps select c.CustomerAppId).ToList();
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this, c => authorizedCustomerApps.Contains(c.CustomerAppId));

            //Customer dependant entities
            var authorizedCustomerIds = ResolveAuthorizedCustomersByAuthorizedLicenses()
                                    .Union(ResolveAuthorizedCustomersByUser(currentUser));
            this.Customers = new FilteredDbSet<Customer>(this, c => authorizedCustomerIds.Contains(c.ObjectId));
        }

        /// <summary>
        /// Gets a datacontext based on single transaction ID
        /// </summary>
        /// <returns>Returns a single transaction access datacontext</returns>
        public DataContext(int transactionID)
            : base()
        {
            this.Transactions = new FilteredDbSet<Transaction>(this, x => x.TransactionId == transactionID);
            this.TransactionItems = new FilteredDbSet<TransactionItem>(this, x => x.TransactionId == transactionID);

            var authorizedSKUs = (from t in TransactionItems select t.SkuId).ToList();
            this.SKUs = new FilteredDbSet<SKU>(this, x => authorizedSKUs.Contains(x.SkuId));

            var authorizedLicenses = (from t in TransactionItems select t.LicenseId).ToList();
            this.Licenses = new FilteredDbSet<License>(this, x => authorizedLicenses.Contains(x.ObjectId));

            //No access to other entities
            this.Vendors = new FilteredDbSet<Vendor>(this, x => false);
            this.Features = new FilteredDbSet<Feature>(this, x => false);
            this.Customers = new FilteredDbSet<Customer>(this, x => false);
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this, x => false);
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this, x => false);
        }

        /// <summary>
        /// Gets a transaction write only datacontext
        /// </summary>
        /// <returns>Returns a single transaction access datacontext</returns>
        public DataContext(bool transactionID)
            : base()
        {
            this.Transactions = new FilteredDbSet<Transaction>(this);
            this.TransactionItems = new FilteredDbSet<TransactionItem>(this);

            this.Vendors = new FilteredDbSet<Vendor>(this, x => false);
            this.SKUs = new FilteredDbSet<SKU>(this, x => false);
            this.Features = new FilteredDbSet<Feature>(this, x => false);
            this.Licenses = new FilteredDbSet<License>(this, x => false);
            this.Customers = new FilteredDbSet<Customer>(this, x => false);
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this, x => false);
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this, x => false);
        }

        /// <summary>
        /// Gets a datacontext based on full administrator rights
        /// </summary>
        /// <returns>Returns an all access datacontext</returns>
        public DataContext()
            : base()
        {
            this.Vendors = new FilteredDbSet<Vendor>(this);
            this.SKUs = new FilteredDbSet<SKU>(this);
            this.Features = new FilteredDbSet<Feature>(this);
            this.Licenses = new FilteredDbSet<License>(this);
            this.TransactionItems = new FilteredDbSet<TransactionItem>(this);
            this.Customers = new FilteredDbSet<Customer>(this);
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this);
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this);
            this.Transactions = new FilteredDbSet<Transaction>(this);
        }

        public User GetUserByIdentity(IIdentity userIdentity)
        {
            User currentUser = null;
            if (userIdentity.IsAuthenticated)
            {
                currentUser = (from x in this.Users where x.UserName == userIdentity.Name select x).Include(x => x.Rights).FirstOrDefault();
            }

            if (currentUser != null)
                return currentUser;
            else
                // Unauthenticated user or authenticated user not found
                return new User();
        }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OpenAuthUser> OpenAuthUsers { get; set; }

        public DbSet<Country> Countries { get; set; }

        public IDbSet<Feature> Features { get; set; }

        public DbSet<PrivateKey> PrivateKeys { get; set; }

        public IDbSet<SKU> SKUs { get; set; }

        public IDbSet<Transaction> Transactions { get; set; }

        public IDbSet<TransactionItem> TransactionItems { get; set; }

        public IDbSet<Vendor> Vendors { get; set; }

        public IDbSet<Customer> Customers { get; set; }

        public IDbSet<License> Licenses { get; set; }

        public DbSet<Right> Rights { get; set; }

        public DbSet<UserVendorRight> UserVendorRights { get; set; }

        public DbSet<UserCustomerRight> UserCustomerRights { get; set; }

        public DbSet<UserLicenseRight> UserLicenseRights { get; set; }

        public DbSet<DomainLicense> DomainLicenses { get; set; }

        public IDbSet<CustomerApp> CustomerApps { get; set; }

        public IDbSet<LicenseCustomerApp> LicenseCustomerApps { get; set; }

        public DbSet<CustomerAppKey> CustomerAppKeys { get; set; }

        /// <summary>
        /// Get all IEntityConfiguration classes and register the configuration classes
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            foreach (var modelConfiguration in DependencyContext.Instance.GetExportedValues<IEntityConfiguration>())
            {
                modelConfiguration.AddConfiguration(modelBuilder.Configurations);
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Saves changes to the datacontext while providing an action for validation results.
        /// </summary>
        /// <param name="validationFailedAction">The action to be executed when validation has failed.</param>
        public Boolean SaveChanges(Action<BusinessRuleValidationException> validationFailedAction)
        {
            try
            {
                SaveChanges();
                return true;
            }
            catch (BusinessRuleValidationException ex)
            {
                validationFailedAction(ex);
            }
            return false;
        }

        /// <summary>
        /// Overridden validationresult to include 
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            IModelItem entity = entityEntry.Entity as IModelItem;

            if (entity != null)
            {
                var validationResults = BusinessRuleExecutor.ExecuteBusinessResult<DataContext>(entity, this, entityEntry);
                if (!validationResults.All(x => x == BusinessRuleValidationResult.Success))
                {
                    throw new BusinessRuleValidationException(validationResults.ToArray());
                }
            }

            return base.ValidateEntity(entityEntry, items);
        }

        #region "Resolving user rights"
        /// <summary>
        /// Resolve vendor rights based on current user
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedVendorsByUser(User currentUser)
        {
            if (currentUser.IsSystemAdmin)
                return (from x in this.Set<Vendor>() select x.ObjectId).ToList();
            else
                return (from r in currentUser.Rights where r is UserVendorRight && r.RightId == VendorAdmin.Id select r.ObjectId).ToList();
        }

        /// <summary>
        /// Resolve customer rights based on current user
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedCustomersByUser(User currentUser)
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
        private IEnumerable<Guid> ResolveAuthorizedLicensesByUser(User currentUser)
        {
            var authorizedVendorIds = ResolveAuthorizedVendorsByUser(currentUser);
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
        private IEnumerable<Guid> ResolveAuthorizedSKUsByAuthorizedVendors()
        {
            if (Vendors == null)
                throw new DbSetNotReadyException("Unable to resolve authorized SKUs by authorized vendors, vendor DbSet is not set!");

            var authorizedVendorIds = (from v in this.Vendors select v.ObjectId).ToList();
            return (from s in this.Set<SKU>() where authorizedVendorIds.Contains(s.VendorId) select s.SkuId).ToList();
        }

        /// <summary>
        /// Based on the current set of licenses resolve skus
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedSKUsByAuthorizedLicenses()
        {
            if (Licenses == null)
                throw new DbSetNotReadyException("Unable to resolve authorized SKUs by authorized licenses, license DbSet is not set!");

            return (from l in this.Licenses select l.SkuId).ToList();
        }

        /// <summary>
        /// Based on the current set of transaction items resolve transactions
        /// </summary>
        private IEnumerable<int> ResolveAuthorizedTransactionsByAuthorizedTransactionItems()
        {
            if (TransactionItems == null)
                throw new DbSetNotReadyException("Unable to resolve authorized transactions by authorized transaction items, transaction items DbSet is not set!");

            return (from t in this.TransactionItems select t.TransactionId).ToList();
        }

        /// <summary>
        /// Based on the current set of licenses resolve customers
        /// </summary>
        private IEnumerable<Guid> ResolveAuthorizedCustomersByAuthorizedLicenses()
        {
            if (Licenses == null)
                throw new DbSetNotReadyException("Unable to resolve authorized customers by authorized licenses, license DbSet is not set!");

            return (from l in this.Licenses select l.PurchasingCustomerId).ToList()
                   .Union
                   (from l in this.Licenses select l.OwningCustomerId).ToList();
        }
        #endregion
    }

    public class DbSetNotReadyException : Exception
    {
        public DbSetNotReadyException(string message)
            : base(message)
        {
        }
    }
}