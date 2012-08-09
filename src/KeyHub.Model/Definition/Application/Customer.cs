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
    /// Represents the basic buying entity for KeyHub. A Customer can either be a Reseller or an Owner.
    /// </summary>
    public class Customer : RightObject
    {
        /// <summary>
        /// The name of the customer (single line)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// The department this Customer is buying for/from
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Department { get; set; }

        /// <summary>
        /// The street this customer is located at.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// The postal code this customer is located in.
        /// </summary>
        [Required]
        [StringLength(24)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city this customer is located in.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// The region this customer is located in (for Americans it's the state, for Dutch it's province)
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
        /// Paypal ID for this customer
        /// </summary>
        [StringLength(256)]
        public string PayPalId { get; set; }
    }
}