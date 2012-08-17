using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Vendor
{
    /// <summary>
    /// Viewmodel for creation of a Vendor
    /// </summary>
    public class VendorCreateViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="countries">Country list for selectionlist</param>
        public VendorCreateViewModel(List<Model.Country> countries)
        {
            Vendor = new VendorViewModel(new Model.Vendor());
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
        /// <returns>New vendor containing viewmodel data </returns>
        public override Model.Vendor ToEntity(Model.Vendor original)
        {
            return Vendor.ToEntity(null);
        }
    }
}