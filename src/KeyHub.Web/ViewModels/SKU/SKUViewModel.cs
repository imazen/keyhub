using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.SKU
{
    /// <summary>
    /// Viewmodel for a single SKU 
    /// </summary>
    public class SKUViewModel : RedirectUrlModel
    {
        public SKUViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="sku">SKU that this viewmodel represents</param>
        public SKUViewModel(Model.SKU sku)
            : this()
        {
            //Load properties into viewmodel
            SkuId = sku.SkuId;
            VendorId = sku.VendorId;
            PrivateKeyId = sku.PrivateKeyId;
            SkuCode = sku.SkuCode;
            SkuAternativeCode = sku.SkuAternativeCode;
            MaxDomains = sku.MaxDomains;
            EditOwnershipDuration = sku.EditOwnershipDuration;
            MaxSupportContacts = sku.MaxSupportContacts;
            EditSupportContactsDuration = sku.EditSupportContactsDuration;
            LicenseDuration = sku.LicenseDuration;
            AutoDomainDuration = sku.AutoDomainDuration;
            ManualDomainDuration = sku.ManualDomainDuration;
            CanDeleteAutoDomains = sku.CanDeleteAutoDomains;
            CanDeleteManualDomains = sku.CanDeleteManualDomains;
            ReleaseDate = sku.ReleaseDate;
            ExpirationDate = sku.ExpirationDate;
        }

        /// <summary>
        /// Convert back to SKU instance
        /// </summary>
        /// <param name="original">Original SKU. If Null a new instance is created.</param>
        /// <returns>SKU containing viewmodel data </returns>
        public Model.SKU ToEntity(Model.SKU original)
        {
            Model.SKU current = original ?? new Model.SKU();

            //Load properties into entity
            current.SkuId = this.SkuId;
            current.VendorId = this.VendorId;
            current.PrivateKeyId = this.PrivateKeyId;
            current.SkuCode = this.SkuCode;
            current.SkuAternativeCode = this.SkuAternativeCode;
            current.MaxDomains = this.MaxDomains;
            current.EditOwnershipDuration = this.EditOwnershipDuration;
            current.MaxSupportContacts = this.MaxSupportContacts;
            current.EditSupportContactsDuration = this.EditSupportContactsDuration;
            current.LicenseDuration = this.LicenseDuration;
            current.AutoDomainDuration = this.AutoDomainDuration;
            current.ManualDomainDuration = this.ManualDomainDuration;
            current.CanDeleteAutoDomains = this.CanDeleteAutoDomains;
            current.CanDeleteManualDomains = this.CanDeleteManualDomains;
            current.ReleaseDate = this.ReleaseDate;
            current.ExpirationDate = this.ExpirationDate;

            return current;
        }

        /// <summary>
        /// Indentifier for the SKU entity
        /// </summary>
        [DisplayName("Sku key")]
        public Guid SkuId { get; set; }

        /// <summary>
        /// The vendor for this SKU
        /// </summary>
        [Required]
        [DisplayName("Vendor")]
        public Guid VendorId { get; set; }

        /// <summary>
        /// The private key for this SKU
        /// </summary>
        [Required]
        [DisplayName("PrivateKey")]
        public Guid PrivateKeyId { get; set; }

        /// <summary>
        /// The unique (per vendor) SKU indentifier string
        /// </summary>
        [Required]
        [StringLength(256)]
        [DisplayName("Name")]
        public string SkuCode { get; set; }

        /// <summary>
        /// An alternative unique (per vendor) SKU indentifier string
        /// </summary>
        [StringLength(256)]
        [DisplayName("Alternative name")]
        public string SkuAternativeCode { get; set; }

        /// <summary>
        /// The maximum number of domain licenses permitted by this license.
        /// Leave empty to disable maximum on domains
        /// </summary>
        [DisplayName("Max domains")]
        public int? MaxDomains { get; set; }

        /// <summary>
        /// How long the owner fields are editable after license is issued (in days)
        /// Leave empty to disable changing the ownership
        /// </summary>
        [DisplayName("EditOwnershipduration (days)")]
        public int? EditOwnershipDuration { get; set; }

        /// <summary>
        /// Maxmimum number of users listed as a support contact.
        /// Leave empty to disable support contrats.
        /// </summary>
        public int? MaxSupportContacts { get; set; }

        /// <summary>
        /// How long the assigned support contact can be changed
        /// Leave empty to disable changing the duration.
        /// </summary>
        [DisplayName("EditSupportContactsDuration (days)")]
        public int? EditSupportContactsDuration { get; set; }

        /// <summary>
        /// How long the license is valid for
        /// Leave empty to expiration on SKU.
        /// </summary>
        [DisplayName("Licenseduration (months)")]
        public int? LicenseDuration { get; set; }

        /// <summary>
        /// How long auto-generated domain licenses are valid before they must be auto-renewed
        /// Leave empty to disable auto domains.
        /// </summary>
        [DisplayName("AutoDomainDuration (months)")]
        public int? AutoDomainDuration { get; set; }

        /// <summary>
        /// How long manually generated domain licenses are valid for.
        /// Leave empty to disable manual domains.
        /// </summary>
        [DisplayName("ManualDomainDuration (months)")]
        public int? ManualDomainDuration { get; set; }

        /// <summary>
        /// If true, users can delete auto-generated licenses to make room for more or do cleanup
        /// </summary>
        [Required]
        public bool CanDeleteAutoDomains { get; set; }

        /// <summary>
        /// If true, users can delete manual licenses to make room for more or do cleanup
        /// </summary>
        [Required]
        public bool CanDeleteManualDomains { get; set; }

        /// <summary>
        /// When this SKU is first offered for purchase.
        /// Leave empty to disable this SKU
        /// </summary>
        [DisplayName("Released")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// When this SKU is no longer available for purchase
        /// Leave empty to disable expiration on this SKU
        /// </summary>
        [DisplayName("Expires")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime? ExpirationDate { get; set; }
    }
}