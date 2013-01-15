using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

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
        public VendorIndexViewModel(List<Model.Vendor> vendorList):this()
        {
            Vendors = new List<VendorIndexViewItem>();
            foreach (Model.Vendor entity in vendorList)
            {
                Vendors.Add(new VendorIndexViewItem(entity, entity.Country));
            }
        }

        /// <summary>
        /// List of vendors
        /// </summary>
        public List<VendorIndexViewItem> Vendors { get; set; }

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


        /// <summary>
        /// Vendor Id
        /// </summary>
        [DisplayName("Vendor Id")]
        public Guid ObjectId { get; set; }
    }
}