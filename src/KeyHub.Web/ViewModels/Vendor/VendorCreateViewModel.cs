using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels
{
    /// <summary>
    /// Viewmodel for Vendor
    /// </summary>
    public class VendorCreateViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor entity</param>
        /// <param name="countries">Country query for selectionlist</param>
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
        public Model.Vendor ToEntity()
        {
            return Vendor.ToEntity(null);
        }
    }
}