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
    public class DomainLicenseIndexViewModel : BaseViewModel<Model.DomainLicense>
    {
        public DomainLicenseIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="licenseList">List of DomainLicense entities</param>
        public DomainLicenseIndexViewModel(List<Model.DomainLicense> licenseList)
            : this()
        {
            DomainLicenses = new List<DomainLicenseViewModel>(
                    licenseList.Select(x => new DomainLicenseViewModel(x))
                );
        }

        /// <summary>
        /// List of licenses
        /// </summary>
        public List<DomainLicenseViewModel> DomainLicenses { get; set; }

        /// <summary>
        /// Convert back to DomainLicense instance
        /// </summary>
        /// <param name="original">Original DomainLicense. If Null a new instance is created.</param>
        /// <returns>DomainLicense containing viewmodel data </returns>
        public override Model.DomainLicense ToEntity(Model.DomainLicense original)
        {
            throw new NotImplementedException();
        }
    }
}