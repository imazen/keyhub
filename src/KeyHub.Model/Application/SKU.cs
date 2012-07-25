using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// An SKU is a purchasable item that you can add to the shopping cart,
    /// like the Performance Enterprise, Performance Professional, or Cloud Enterprise bundles.
    /// It combines licensing rules with feature codes and packages.
    /// </summary>
    public class SKU
    {
        /// <summary>
        /// Indentifier for the SKU entity
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid SkuId { get; set; }

        /// <summary>
        /// The vendor for this SKU
        /// </summary>
        [Required]
        public Guid VendorId { get; set; }

        /// <summary>
        /// The vendor for this SKU
        /// </summary>
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }

        /// <summary>
        /// The private key for this SKU
        /// </summary>
        [Required]
        public Guid PrivateKeyId { get; set; }

        /// <summary>
        /// The private key for this SKU
        /// </summary>
        [ForeignKey("PrivateKeyId")]
        public virtual PrivateKey PrivateKey { get; set; }

        /// <summary>
        /// The unique (per vendor) SKU indentifier string
        /// </summary>
        [Required]
        [StringLength(256)]
        [Column(TypeName = "varchar")]
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
        [Column(TypeName = "datetime2")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// When this SKU is no longer available for purchase
        /// Leave empty to disable expiration on this SKU
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// The list of features this SKU consists of.
        /// </summary>
        public virtual ICollection<SkuFeature> SkuFeatures { get; set; }
    }
}