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
    public class DomainLicenseCreateViewModel : RedirectUrlModel
    {
        public DomainLicenseCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="license">Licenses for domain</param>
        public DomainLicenseCreateViewModel(Model.License license)
        {
            var domainLicense = new Model.DomainLicense()
            {
                LicenseId = license.ObjectId,
                License = license,
                DomainLicenseIssued = license.Sku.CalculateDomainIssueDate()
            };

            if (license.Sku.CanCalculateManualDomainExpiration)
            {
                domainLicense.DomainLicenseExpires = license.Sku.CalculateManualDomainExpiration();
            }

            DomainLicense = new DomainLicenseViewModel(domainLicense);
        }

        /// <summary>
        /// Edited DomainLicense
        /// </summary>
        public DomainLicenseViewModel DomainLicense { get; set; }

        /// <summary>
        /// Convert back to DomainLicense instance
        /// </summary>
        /// <returns>New DomainLicense containing viewmodel data </returns>
        public Model.DomainLicense ToEntity(Model.DomainLicense original)
        {
            return DomainLicense.ToEntity(null);
        }
    }
}