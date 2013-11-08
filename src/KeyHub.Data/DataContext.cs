using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Security.Principal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Common;
using KeyHub.Core.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using System.Web;

namespace KeyHub.Data
{
    /// <summary>
    /// Provides data access to all tables and collections
    /// </summary>
    public class DataContext : DbContext, IDataContext
    {
        /// <summary>
        /// Gets or sets the BusinessRuleExecutorFactory
        /// </summary>
        public IBusinessRuleExecutorFactory BusinessRuleExecutorFactory { get; set; }

        /// <summary>
        /// Gets a datacontext based on full administrator rights
        /// </summary>
        /// <returns>Returns an all access datacontext</returns>
        public DataContext()
            : base(Constants.ConnectionStringName)
        {
            this.Vendors = new FilteredDbSet<Vendor>(this);
            this.VendorCredentials = new FilteredDbSet<VendorCredential>(this);
            this.SKUs = new FilteredDbSet<SKU>(this);
            this.Features = new FilteredDbSet<Feature>(this);
            this.SkuFeatures = new FilteredDbSet<SkuFeature>(this);
            this.Licenses = new FilteredDbSet<License>(this);
            this.TransactionItems = new FilteredDbSet<TransactionItem>(this);
            this.TransactionIgnoredItems = new FilteredDbSet<TransactionIgnoredItem>(this);
            this.Customers = new FilteredDbSet<Customer>(this);
            this.LicenseCustomerApps = new FilteredDbSet<LicenseCustomerApp>(this);
            this.CustomerApps = new FilteredDbSet<CustomerApp>(this);
            this.CustomerAppIssues = new FilteredDbSet<CustomerAppIssue>(this);
            this.Transactions = new FilteredDbSet<Transaction>(this);
        }

        /// <summary>
        /// Gets the user by its current identity
        /// </summary>
        /// <param name="identity">Identity of the user</param>
        /// <returns>Currently logged in use</returns>
        public User GetUser(IIdentity identity)
        {
            User currentUser = null;
            if (identity.IsAuthenticated)
            {
                currentUser = (from x in this.Users where x.MembershipUserIdentifier == identity.Name select x).Include(x => x.Rights).FirstOrDefault();
            }

            return currentUser ?? new User();
        }

        public IDbSet<Membership> Memberships { get; set; }

        public IDbSet<Role> Roles { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<Country> Countries { get; set; }

        public IDbSet<Feature> Features { get; set; }

        public IDbSet<PrivateKey> PrivateKeys { get; set; }

        public IDbSet<SKU> SKUs { get; set; }

        public IDbSet<SkuFeature> SkuFeatures { get; set; }

        public IDbSet<Transaction> Transactions { get; set; }

        public IDbSet<TransactionItem> TransactionItems { get; set; }

        public IDbSet<TransactionIgnoredItem> TransactionIgnoredItems { get; set; }

        public IDbSet<Vendor> Vendors { get; set; }

        public IDbSet<VendorCredential> VendorCredentials { get; set; }

        public IDbSet<Customer> Customers { get; set; }

        public IDbSet<License> Licenses { get; set; }

        public IDbSet<Right> Rights { get; set; }

        public IDbSet<UserVendorRight> UserVendorRights { get; set; }

        public IDbSet<UserCustomerRight> UserCustomerRights { get; set; }

        public IDbSet<UserLicenseRight> UserLicenseRights { get; set; }

        public IDbSet<DomainLicense> DomainLicenses { get; set; }

        public IDbSet<CustomerApp> CustomerApps { get; set; }

        public IDbSet<CustomerAppIssue> CustomerAppIssues { get; set; }

        public IDbSet<LicenseCustomerApp> LicenseCustomerApps { get; set; }

        public IDbSet<CustomerAppKey> CustomerAppKeys { get; set; }

        /// <summary>
        /// Get all IEntityConfiguration classes and register the configuration classes
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Create the composition container
            using (var container = new CompositionContainer(new AssemblyCatalog(GetType().Assembly),
                                                            CompositionOptions.DisableSilentRejection))
            {
                foreach (var modelConfiguration in container.GetExportedValues<IEntityConfiguration>())
                {
                    modelConfiguration.AddConfiguration(modelBuilder.Configurations);
                }
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
            var entity = entityEntry.Entity as IModelItem;

            if (entity != null)
            {
                ValidateModelItem(entity, entityEntry);
            }

            return base.ValidateEntity(entityEntry, items);
        }

        /// <summary>
        /// Validates entity for unit tests only.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityEntry">It can be null if it is enough for business rules of current entity.</param>
        public void ValidateModelItem(IModelItem entity, DbEntityEntry entityEntry = null)
        {
            if (BusinessRuleExecutorFactory == null)
                return;

            var executor = BusinessRuleExecutorFactory.Create();
            var validationResults = executor.ExecuteBusinessResult(entity, entityEntry).ToArray();

            if (validationResults.Any(x => x != BusinessRuleValidationResult.Success))
            {
                throw new BusinessRuleValidationException(validationResults);
            }
        }

        /// <summary>
        /// Indicates if entity should validate
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <returns></returns>
        protected override bool ShouldValidateEntity(DbEntityEntry entityEntry)
        {
            if (entityEntry.Entity is IModelItem)
            {
                return true;
            }

            return base.ShouldValidateEntity(entityEntry);
        }
    }
}