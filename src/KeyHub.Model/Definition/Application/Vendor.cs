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
    /// Vendor entity to physically split company plugins (Lucrasoft and Imazen)
    /// </summary>
    public class Vendor : RightObject
    {
        /// <summary>
        /// The public name for this vendor/organisation
        /// </summary>
        [Required]
        [StringLength(512)]
        public string OrganisationName { get; set; }

        /// <summary>
        /// The street this vendor is located at.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// The postal code this vendor is located in.
        /// </summary>
        [Required]
        [StringLength(24)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city this vendor is located in.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// The region this vendor is located in (for Americans it's the state, for Dutch it's province)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Region { get; set; }

        /// <summary>
        /// CountryCode of the customer.
        /// </summary>
        [Required]
        [StringLength(12)]
        [Column(TypeName = "varchar")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Country of the customer. We use the English name from the RegionInfo classes as identifier
        /// </summary>
        [ForeignKey("CountryCode")]
        public Country Country { get; set; }

        /// <summary>
        /// A list of private keys this vendor owns
        /// </summary>
        public virtual ICollection<PrivateKey> PrivateKeys { get; set; }
    }
}