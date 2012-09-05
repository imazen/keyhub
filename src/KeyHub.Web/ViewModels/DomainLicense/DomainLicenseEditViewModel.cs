using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.DomainLicense
{
    /// <summary>
    /// Viewmodel for creation of a DomainLicense
    /// </summary>
    public class DomainLicenseEditViewModel : BaseViewModel<Model.DomainLicense>
    {
        public DomainLicenseEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="licenses">Licenses for selectionlist</param>
        public DomainLicenseEditViewModel(List<Model.License> licenses)
        {
            DomainLicense = new DomainLicenseViewModel(new Model.DomainLicense());
            LicenseList = licenses.ToSelectList(v => v.ObjectId, v => v.Sku.SkuCode);
        }

        /// <summary>
        /// Edited DomainLicense
        /// </summary>
        public DomainLicenseViewModel DomainLicense { get; set; }

        /// <summary>
        /// List of licenses to select
        /// </summary>
        public SelectList LicenseList { get; set; }

        /// <summary>
        /// Convert back to DomainLicense instance
        /// </summary>
        /// <returns>New DomainLicense containing viewmodel data </returns>
        public override Model.DomainLicense ToEntity(Model.DomainLicense original)
        {
            return DomainLicense.ToEntity(null);
        }
    }
}