using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for detail of Licenses
    /// </summary>
    public class LicenseDetailsViewModel : BaseViewModel<Model.License>
    {
        public LicenseDetailsViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="license">License entity to show</param>
        public LicenseDetailsViewModel(Model.License license)
            : this()
        {
            License = new LicenseDetailsViewItem(license, license.PurchasingCustomer, license.OwningCustomer, license.Sku);
        }

        /// <summary>
        /// License
        /// </summary>
        public LicenseDetailsViewItem License { get; set; }

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
    /// LicenseViewModel extension that includes Owning Customer and Purchasing Customer name
    /// </summary>
    public class LicenseDetailsViewItem : LicenseViewModel
    {
        public LicenseDetailsViewItem(Model.License license, Model.Customer purchasingCustomer, Model.Customer owningCustomer, Model.SKU sku)
            : base(license)
        {
            OwningCustomerName = (owningCustomer != null) ? owningCustomer.Name : "Not owned";
            PurchasingCustomerName = purchasingCustomer.Name;
            SKUName = sku.SkuCode;
        }

        /// <summary>
        /// Name of the customer that purchased the license
        /// </summary>
        [DisplayName("Purchased by")]
        public string PurchasingCustomerName { get; set; }

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
    }
}