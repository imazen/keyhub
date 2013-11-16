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
        /// <summary>
        /// Edited DomainLicense
        /// </summary>
        public DomainLicenseViewModel DomainLicense { get; set; }
    }
}