using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.Vendor
{
    /// <summary>
    /// Viewmodel for details a Vendor
    /// </summary>
    public class VendorDetailsViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorDetailsViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor entity</param>
        public VendorDetailsViewModel(Model.Vendor vendor)
            : this()
        {
            Vendor = new VendorDetailsViewItem(vendor, vendor.Country);
        }

        /// <summary>
        /// Vendor viewmodel
        /// </summary>
        public VendorDetailsViewItem Vendor { get; set; }

        /// <summary>
        /// Convert back to Vendor instance
        /// </summary>
        /// <param name="original">Original Vendor. If Null a new instance is created.</param>
        /// <returns>Vendor containing viewmodel data </returns>
        public override Model.Vendor ToEntity(Model.Vendor original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// VendorViewModel extension that includes the name of the countrycode assinged country
    /// </summary>
    public class VendorDetailsViewItem : VendorViewModel
    {
        public VendorDetailsViewItem(Model.Vendor vendor, Model.Country country)
            : base(vendor)
        {
            CountryName = country.CountryName;
        }

        /// <summary>
        /// Countryname of the customer.
        /// </summary>
        [DisplayName("Country")]
        public string CountryName { get; set; }
    }
}