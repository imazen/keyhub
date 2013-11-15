using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.DomainLicense
{
    /// <summary>
    /// Viewmodel for index list of DomainLicenses
    /// </summary>
    public class DomainLicenseIndexViewModel : RedirectUrlModel
    {
        public DomainLicenseIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="parentLicense">Guid of the owning license</param>
        /// <param name="licenseList">List of DomainLicense entities</param>
        public DomainLicenseIndexViewModel(Guid parentLicense, IEnumerable<Model.DomainLicense> licenseList)
            : this()
        {
            this.PartentLicense = parentLicense;

            this.DomainLicenses = new List<DomainLicenseIndexViewItem>(
                    licenseList.Select(x => new DomainLicenseIndexViewItem(x))
                );
        }

        /// <summary>
        /// Guid of the parent license
        /// </summary>
        public Guid PartentLicense { get; set; }

        /// <summary>
        /// List of licenses
        /// </summary>
        public List<DomainLicenseIndexViewItem> DomainLicenses { get; set; }
    }

    public class DomainLicenseIndexViewItem : DomainLicenseViewModel
    {
        public DomainLicenseIndexViewItem() : base() {}

        public DomainLicenseIndexViewItem(Model.DomainLicense domain)
            : base(domain)
        {
            
        }
    }
}