using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.DomainLicense
{
    /// <summary>
    /// Viewmodel for a single domainLicense 
    /// </summary>
    public class DomainLicenseViewModel : BaseViewModel<Model.DomainLicense>
    {
        public DomainLicenseViewModel():base(){ }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="domainLicense">DomainLicense that this viewmodel represents</param>
        public DomainLicenseViewModel(Model.DomainLicense domainLicense)
            : this()
        {
            this.DomainLicenseId = domainLicense.DomainLicenseId;
            this.LicenseId = domainLicense.LicenseId;
            this.LicenseName = domainLicense.License.Sku.SkuCode;
            this.DomainName = domainLicense.DomainName;
            this.DomainLicenseIssued = domainLicense.DomainLicenseIssued;
            this.DomainLicenseExpires = domainLicense.DomainLicenseExpires;
            this.AutomaticlyCreated = domainLicense.AutomaticlyCreated;
            this.CanBeManuallyDeleted = domainLicense.CanBeManuallyDeleted;
            this.CanCalculateDomainExpiration = domainLicense.License.Sku.CanCalculateManualDomainExpiration;
        }

        /// <summary>
        /// Convert back to DomainLicense instance
        /// </summary>
        /// <param name="original">Original DomainLicense. If Null a new instance is created.</param>
        /// <returns>DomainLicense containing viewmodel data </returns>
        public override Model.DomainLicense ToEntity(Model.DomainLicense original)
        {
            if (original == null)
            {
                original = new Model.DomainLicense();
            }
            var current = original;

            current.DomainLicenseId = this.DomainLicenseId;
            current.LicenseId = this.LicenseId;
            current.DomainName = this.DomainName;
            current.DomainLicenseIssued = this.DomainLicenseIssued;
            current.DomainLicenseExpires = this.DomainLicenseExpires;
            current.AutomaticlyCreated = this.AutomaticlyCreated;

            return current;
        }

        /// <summary>
        /// Unique LicenseDomainId as identity for a domain
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid DomainLicenseId { get; set; }

        /// <summary>
        /// The license this domains is associated with
        /// </summary>
        [Required]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// The domainname for this LicenseDomainRecord
        /// </summary>
        [Required]
        [StringLength(256)]
        public string DomainName { get; set; }

        /// <summary>
        /// The date this domain has been created (either automaticly or manual)
        /// Manually created licenses may or may not have an expiration date. 
        /// If there is no expiration, it should be null.
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime DomainLicenseIssued { get; set; }

        /// <summary>
        /// The date this domain license will expire
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime? DomainLicenseExpires { get; set; }

        /// <summary>
        /// Should be true if automatically generated
        /// </summary>
        [Required]
        public bool AutomaticlyCreated { get; set; }

        /// <summary>
        /// The license name this domains is associated with
        /// </summary>
        [DisplayName("License")]
        public string LicenseName { get; set; }

        /// <summary>
        /// Check if domain license can be deleted
        /// </summary>
        public bool CanBeManuallyDeleted { get; set; }

        /// <summary>
        /// Check if server can calculate domain expiration, therefore user do not needed to input expiration date
        /// </summary>
        public bool CanCalculateDomainExpiration { get; set; }
    }
}