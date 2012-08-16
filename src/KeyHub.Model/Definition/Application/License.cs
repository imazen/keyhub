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
    /// Represents a purchased SKU for storing owner and license information
    /// </summary>
    public class License : RightObject
    {
        /// <summary>
        /// The SKU bought with this license
        /// </summary>
        [Required]
        public Guid SkuId { get; set; }

        /// <summary>
        /// The SKU bought with this license
        /// </summary>
        [ForeignKey("SkuId")]
        public virtual SKU Sku { get; set; }

        /// <summary>
        /// The Customer that purchased this license
        /// </summary>
        [Required]
        public Guid PurchasingCustomerId { get; set; }

        /// <summary>
        /// The Customer that purchased this license
        /// </summary>
        [ForeignKey("PurchasingCustomerId")]
        public virtual Customer PurchasingCustomer { get; set; }

        /// <summary>
        /// The original name of the purchasing entity that bought this license.
        /// Value cannot be edited after SKU.EditOwnershipDuration has expired.
        /// </summary>
        [Required]
        [StringLength(1024)]
        public string OwnerName { get; set; }

        /// <summary>
        /// The Customer that ownes this license
        /// </summary>
        /// <remarks>Can be empty if license is unassigned?</remarks>
        public Guid OwningCustomerId { get; set; }

        /// <summary>
        /// The Customer that ownes this license
        /// </summary>
        [ForeignKey("OwningCustomerId")]
        public virtual Customer OwningCustomer { get; set; }

        /// <summary>
        /// The date this license has been issued
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime LicenseIssued { get; set; }

        /// <summary>
        /// The date this license will expire
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? LicenseExpires { get; set; }

        /// <summary>
        /// A list of domains <see cref="DomainLicense"/> associated with this license
        /// </summary>
        public virtual ICollection<DomainLicense> Domains { get; set; }

        /// <summary>
        /// The TransactionItem that resulted in the purchase of the license
        /// </summary>
        public virtual ICollection<TransactionItem> TransactionItems { get; set; }
    }
}