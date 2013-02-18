using System;
using System.Data.Entity;
using System.Security.Principal;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;

namespace KeyHub.Data
{
    public interface IDataContext : IDisposable
    {
        IDbSet<Membership> Memberships { get; set; }
        IDbSet<Role> Roles { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<Country> Countries { get; set; }
        IDbSet<Feature> Features { get; set; }
        IDbSet<PrivateKey> PrivateKeys { get; set; }
        IDbSet<SKU> SKUs { get; set; }
        IDbSet<Transaction> Transactions { get; set; }
        IDbSet<TransactionItem> TransactionItems { get; set; }
        IDbSet<Vendor> Vendors { get; set; }
        IDbSet<Customer> Customers { get; set; }
        IDbSet<License> Licenses { get; set; }
        IDbSet<Right> Rights { get; set; }
        IDbSet<UserVendorRight> UserVendorRights { get; set; }
        IDbSet<UserCustomerRight> UserCustomerRights { get; set; }
        IDbSet<UserLicenseRight> UserLicenseRights { get; set; }
        IDbSet<DomainLicense> DomainLicenses { get; set; }
        IDbSet<CustomerApp> CustomerApps { get; set; }
        IDbSet<LicenseCustomerApp> LicenseCustomerApps { get; set; }
        IDbSet<CustomerAppKey> CustomerAppKeys { get; set; }

        /// <summary>
        /// Saves changes to the datacontext while providing an action for validation results.
        /// </summary>
        /// <param name="validationFailedAction">The action to be executed when validation has failed.</param>
        Boolean SaveChanges(Action<BusinessRuleValidationException> validationFailedAction);

        /// <summary>
        /// Saves changes to the datacontext
        /// </summary>
        /// <returns>Nr of rows affected</returns>
        int SaveChanges();

        /// <summary>
        /// Gets the user by its current identity
        /// </summary>
        /// <param name="identity">Identity of the user</param>
        /// <returns>Currently logged in use</returns>
        User GetUser(IIdentity identity);
    }
}
