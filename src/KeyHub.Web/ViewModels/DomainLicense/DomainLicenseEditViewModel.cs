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
        /// <param name="domainLicense">Domain license to edit</param>
        public DomainLicenseEditViewModel(Model.DomainLicense domainLicense)
        {
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
        public override Model.DomainLicense ToEntity(Model.DomainLicense original)
        {
            return DomainLicense.ToEntity(original);
        }
    }
}