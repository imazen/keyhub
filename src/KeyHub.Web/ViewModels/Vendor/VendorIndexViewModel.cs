using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.ViewModels.Vendor
{
    /// <summary>
    /// Viewmodel for index list of a Vendor
    /// </summary>
    public class VendorIndexViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendorList">List of vendor entities</param>
        public VendorIndexViewModel(Model.User currentUser, List<Model.Vendor> vendorList)
            : this()
        {
            CurrentUser = new CurrentUserViewModel(currentUser);

            Vendors = new List<VendorIndexViewItem>();
            foreach (Model.Vendor entity in vendorList)
            {
                Vendors.Add(new VendorIndexViewItem(entity, entity.Country));
            }
        }

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }

        /// <summary>
        /// List of vendors
        /// </summary>
        public List<VendorIndexViewItem> Vendors { get; set; }
    }

    /// <summary>
    /// VendorViewModel extension that includes the name of the countrycode assinged country
    /// </summary>
    public class VendorIndexViewItem : VendorViewModel
    {
        public VendorIndexViewItem(Model.Vendor vendor, Model.Country country)
            : base(vendor)
        {
            ObjectId = vendor.ObjectId;
            CountryName = country.CountryName;
        }

        /// <summary>
        /// Countryname of the customer.
        /// </summary>
        [DisplayName("Country")]
        public string CountryName { get; set; }
    }
}