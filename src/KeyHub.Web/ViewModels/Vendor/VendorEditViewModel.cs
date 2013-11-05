using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Vendor
{
    /// <summary>
    /// Viewmodel for editing a Vendor
    /// </summary>
    public class VendorEditViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor entity</param>
        /// <param name="countries">Country query for selectionlist</param>
        public VendorEditViewModel(Model.Vendor vendor, List<Model.Country> countries)
        {
            Vendor = new VendorViewModel(vendor);
            CountryList = countries.ToSelectList(c => c.CountryCode, c => c.CountryName);
        }
        
        /// <summary>
        /// Edited vendor
        /// </summary>
        public VendorViewModel Vendor { get; set; }

        /// <summary>
        /// List of countries to select
        /// </summary>
        public SelectList CountryList { get; set; }

        /// <summary>
        /// Convert back to Vendor instance
        /// </summary>
        /// <param name="original">Original Vendor. If Null a new instance is created.</param>
        /// <returns>Vendor containing viewmodel data </returns>
        public override Model.Vendor ToEntity(Model.Vendor original)
        {
            return Vendor.ToEntity(original);
        }
    }
}