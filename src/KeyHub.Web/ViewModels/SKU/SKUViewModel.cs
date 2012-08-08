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
    public class SKUViewModel : BaseViewModel<Model.SKU>
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

            //Fill SkuFeatures list
            SkuFeatures = new List<Feature.FeatureViewModel>
                (
                    sku.SkuFeatures.Select(x => new Feature.FeatureViewModel(x.Feature))
                );

            foreach (Model.SkuFeature skuFeature in sku.SkuFeatures)
            {
                SkuFeatures.Add((new Feature.FeatureViewModel(skuFeature.Feature)));
            }
        }

        /// <summary>
        /// Convert back to SKU instance
        /// </summary>
        /// <param name="original">Original SKU. If Null a new instance is created.</param>
        /// <returns>SKU containing viewmodel data </returns>
        public override Model.SKU ToEntity(Model.SKU original)
        {
            Model.SKU current = original ?? new Model.SKU();

            //Load properties into entity
            current.SkuId = SkuId;
            current.VendorId = VendorId;
            current.PrivateKeyId = PrivateKeyId;
            current.SkuCode = SkuCode;
            current.MaxDomains = MaxDomains;
            current.EditOwnershipDuration = EditOwnershipDuration;
            current.MaxSupportContacts = MaxSupportContacts;
            current.EditSupportContactsDuration = EditSupportContactsDuration;
            current.LicenseDuration = LicenseDuration;
            current.AutoDomainDuration = AutoDomainDuration;
            current.ManualDomainDuration = ManualDomainDuration;
            current.CanDeleteAutoDomains = CanDeleteAutoDomains;
            current.CanDeleteManualDomains = CanDeleteManualDomains;
            current.ReleaseDate = ReleaseDate;
            current.ExpirationDate = ExpirationDate;

            //Add new SkuFeatures to list
            foreach (Feature.FeatureViewModel feature in SkuFeatures)
            {
                var existingFeatures = (from f in current.SkuFeatures where f.FeatureId == feature.FeatureId select f);
                if (existingFeatures.Count() == 0)
                {
                    Model.SkuFeature newFeature = new Model.SkuFeature() { SkuId = current.SkuId, FeatureId= feature.FeatureId };
                    current.SkuFeatures.Add(newFeature);
                }
            }

            //Delete removed SkuFeatures from list
            foreach (Model.SkuFeature feature in current.SkuFeatures)
            {
                var currentFeature = (from f in SkuFeatures where f.FeatureId == feature.FeatureId select f);
                if (currentFeature.Count() == 0)
                {
                    current.SkuFeatures.Remove(feature);
                }
            }

            return current;
        }

        /// <summary>
        /// Indentifier for the SKU entity
        /// </summary>
        [HiddenInput(DisplayValue = false)]
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
        [DisplayName("Code")]
        public string SkuCode { get; set; }

        /// <summary>
        /// The maximum number of domain licenses permitted by this license.
        /// Leave empty to disable maximum on domains
        /// </summary>
        public int? MaxDomains { get; set; }

        /// <summary>
        /// How long the owner fields are editable after license is issued (in days)
        /// Leave empty to disable changing the ownership
        /// </summary>
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
        public int? EditSupportContactsDuration { get; set; }

        /// <summary>
        /// How long the license is valid for
        /// Leave empty to expiration on SKU.
        /// </summary>
        public int? LicenseDuration { get; set; }

        /// <summary>
        /// How long auto-generated domain licenses are valid before they must be auto-renewed
        /// Leave empty to disable auto domains.
        /// </summary>
        public int? AutoDomainDuration { get; set; }

        /// <summary>
        /// How long manually generated domain licenses are valid for.
        /// Leave empty to disable manual domains.
        /// </summary>
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
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// When this SKU is no longer available for purchase
        /// Leave empty to disable expiration on this SKU
        /// </summary>
        [DisplayName("Expires")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// The list of features this SKU consists of.
        /// </summary>
        [DisplayName("Features")]
        public virtual ICollection<Feature.FeatureViewModel> SkuFeatures { get; set; }
    }
}