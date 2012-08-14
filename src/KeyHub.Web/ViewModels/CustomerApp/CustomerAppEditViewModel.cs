using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for editing an CustomerApp
    /// </summary>
    public class CustomerAppEditViewModel : BaseViewModel<Model.CustomerApp>
    {
        public CustomerAppEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="licenses">List of licenses to select</param>
        public CustomerAppEditViewModel(Model.CustomerApp currentCustomerApp, List<Model.License> licenses)
        {
            this.CustomerApp = new CustomerAppEditViewItem(currentCustomerApp);

            this.LicenseList = licenses.ToMultiSelectList(x => x.ObjectId, x => x.Sku.SkuCode);
        }

        /// <summary>
        /// Creating CustomerApp
        /// </summary>
        public CustomerAppEditViewItem CustomerApp { get; set; }

        /// <summary>
        /// List of licenses to select
        /// </summary>
        public MultiSelectList LicenseList { get; set; }

        /// <summary>
        /// Convert back to CustomerApp instance
        /// </summary>
        /// <param name="original">Original CustomerApp. If Null a new instance is edited.</param>
        /// <returns>CustomerApp containing viewmodel data </returns>
        public override Model.CustomerApp ToEntity(Model.CustomerApp original)
        {
            return CustomerApp.ToEntity(original);
        }

        /// <summary>
        /// Retrieve a list of new customer app Licenses
        /// </summary>
        /// <param name="original">Original CustomerApp</param>
        /// <returns>List of assigned license Guids</returns>
        public List<Guid> GetNewLicenseGUIDs(Model.CustomerApp original)
        {
            var originalLicenses = (from x in original.LicenseCustomerApps select x.LicenseId);
            return CustomerApp.SelectedLicenseGUIDs.Except(originalLicenses).ToList();
        }

        /// <summary>
        /// Retrieve a list of removed customer app Licenses
        /// </summary>
        /// <param name="original">Original CustomerApp</param>
        /// <returns>List of removed license Guids</returns>
        public List<Guid> GetRemovedLicenseGUIDs(Model.CustomerApp original)
        {
            var originalLicenses = (from x in original.LicenseCustomerApps select x.LicenseId);
            return originalLicenses.Except(CustomerApp.SelectedLicenseGUIDs).ToList();
        }
    }

    /// <summary>
    /// CustomerAppViewModel extension that contains a list of SelectedLicense Guids
    /// </summary>
    public class CustomerAppEditViewItem : CustomerAppViewModel
    {
        public CustomerAppEditViewItem() : base() 
        {
            SelectedLicenseGUIDs = new List<Guid>();
        }


        public CustomerAppEditViewItem(Model.CustomerApp customerApp)
            : base(customerApp)
        {
            SelectedLicenseGUIDs = new List<Guid>(
                customerApp.LicenseCustomerApps.Select(x => x.LicenseId).ToList()
                );
        }

        /// <summary>
        /// Guid list of assigned licenses
        /// </summary>
        public List<Guid> SelectedLicenseGUIDs { get; set; }
    }
}