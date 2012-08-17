using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for creating an CustomerApp
    /// </summary>
    public class CustomerAppCreateViewModel : BaseViewModel<Model.CustomerApp>
    {
        public CustomerAppCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="licenses">List of licenses to select</param>
        public CustomerAppCreateViewModel(List<Model.License> licenses)
        {
            CustomerApp = new CustomerAppCreateViewItem(new Model.CustomerApp());

            var licenseQuery = from x in licenses select new { ObjectId = x.ObjectId, Name = x.Sku.SkuCode };

            this.LicenseList = licenseQuery.ToList().ToMultiSelectList(x => x.ObjectId, x => x.Name);
        }

        /// <summary>
        /// Creating CustomerApp
        /// </summary>
        public CustomerAppCreateViewItem CustomerApp { get; set; }

        /// <summary>
        /// List of licenses to select
        /// </summary>
        public MultiSelectList LicenseList { get; set; }

        /// <summary>
        /// Convert back to CustomerApp instance
        /// </summary>
        /// <param name="original">Original CustomerApp. If Null a new instance is created.</param>
        /// <returns>CustomerApp containing viewmodel data </returns>
        public override Model.CustomerApp ToEntity(Model.CustomerApp original)
        {
            return CustomerApp.ToEntity(null);
        }

        /// <summary>
        /// Retrieve a list of customer app Licenses
        /// </summary>
        /// <returns>List of assigned license Guids</returns>
        public List<Guid> GetNewLicenseGUIDs()
        {
            return CustomerApp.SelectedLicenseGUIDs.ToList();
        }
    }

    /// <summary>
    /// CustomerAppViewModel extension that contains a list of SelectedLicense Guids
    /// </summary>
    public class CustomerAppCreateViewItem : CustomerAppViewModel
    {
        public CustomerAppCreateViewItem()
            : base()
        {
            SelectedLicenseGUIDs = new List<Guid>();
        }

        public CustomerAppCreateViewItem(Model.CustomerApp customerApp)
            : base(customerApp)
        {
            SelectedLicenseGUIDs = new List<Guid>();
        }

        /// <summary>
        /// Guid list of assigned licenses
        /// </summary>
        public List<Guid> SelectedLicenseGUIDs { get; set; }
    }
}