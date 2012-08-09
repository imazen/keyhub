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
        /// The name of the owner of this license
        /// </summary>
        [Required]
        [StringLength(256)]
        public string OwnerName { get; set; }

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
    }
}