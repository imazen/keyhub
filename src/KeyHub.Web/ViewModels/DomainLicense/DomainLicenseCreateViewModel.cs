using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Web.ViewModels.License;

namespace KeyHub.Web.ViewModels.DomainLicense
{
    /// <summary>
    /// Viewmodel for creation of a DomainLicense
    /// </summary>
    public class DomainLicenseCreateViewModel : BaseViewModel<Model.DomainLicense>
    {
        public DomainLicenseCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="license">Licenses for domain</param>
        public DomainLicenseCreateViewModel(Model.License license)
        {
            DomainLicense = new DomainLicenseViewModel(new Model.DomainLicense() { LicenseId = license.ObjectId, License = license });
        }

        /// <summary>
        /// Edited DomainLicense
        /// </summary>
        public DomainLicenseViewModel DomainLicense { get; set; }

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