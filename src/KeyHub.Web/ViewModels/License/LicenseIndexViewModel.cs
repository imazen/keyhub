using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Data;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for index list of Licenses
    /// </summary>
    public class LicenseIndexViewModel : BaseViewModel<Model.License>
    {
        public LicenseIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="currentUser">Current user</param>
        /// <param name="licenseList">List of License entities</param>
        public LicenseIndexViewModel(Model.User currentUser, IEnumerable<Model.License> licenseList)
            : this()
        {
            CurrentUser = new CurrentUserViewModel(currentUser);

            Licenses = new List<LicenseIndexViewItem>(
                    licenseList.Select(x => new LicenseIndexViewItem(x, x.OwningCustomer, x.Sku, x.Domains))
                );
        }

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }

        /// <summary>
        /// List of licenses
        /// </summary>
        public List<LicenseIndexViewItem> Licenses { get; set; }

        /// <summary>
        /// Convert back to License instance
        /// </summary>
        /// <param name="original">Original License. If Null a new instance is created.</param>
        /// <returns>License containing viewmodel data </returns>
        public override Model.License ToEntity(Model.License original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// LicenseViewModel extension that includes Owning Customer name
    /// </summary>
    public class LicenseIndexViewItem : LicenseViewModel
    {
        public LicenseIndexViewItem(Model.License license, Model.Customer owningCustomer, Model.SKU sku, IEnumerable<Model.DomainLicense> domains)
            : base(license)
        {
            OwningCustomerName = (owningCustomer != null) ? owningCustomer.Name : "Not owned";
            SKUName = sku.SkuCode;
            DomainSummary = domains.ToSummary(x=>x.DomainName, 1, ", ");
        }

        /// <summary>
        /// Name of the customer that ownes the license
        /// </summary>
        [DisplayName("Owner")]
        public string OwningCustomerName { get; set; }

        /// <summary>
        /// Name of the SKU bought with this license
        /// </summary>
        [DisplayName("SKU")]
        public string SKUName { get; set; }

        /// <summary>
        /// Name of the domains registered for this license
        /// </summary>
        [DisplayName("Domains")]
        public string DomainSummary { get; set; }
    }
}