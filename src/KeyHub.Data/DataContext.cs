using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Runtime;

namespace KeyHub.Data
{
    /// <summary>
    /// Provides data access to all tables and collections
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<Application> Applications { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OpenAuthUser> OpenAuthUsers { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<PrivateKey> PrivateKeys { get; set; }

        public DbSet<SKU> SKUs { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionItem> TransactionItems { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<License> Licenses { get; set; }

        public DbSet<Right> Rights { get; set; }

        public DbSet<UserVendorRight> UserVendorRights { get; set; }

        public DbSet<UserCustomerRight> UserCustomerRights { get; set; }

        public DbSet<UserLicenseRight> UserLicenseRights { get; set; }

        public DbSet<DomainLicense> DomainLicenses { get; set; }

        public DbSet<CustomerApp> CustomerApps { get; set; }

        public DbSet<CustomerAppKey> CustomerAppKeys { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Get all IEntityConfiguration classes and register the configuration classes
            foreach (var modelConfiguration in DependencyContext.Instance.GetExportedValues<IEntityConfiguration>())
            {
                modelConfiguration.AddConfiguration(modelBuilder.Configurations);
            }

            base.OnModelCreating(modelBuilder);
        }

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

    }
}