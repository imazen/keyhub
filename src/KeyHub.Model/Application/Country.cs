using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Represents a country. Prefilled from system
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Unique Country code
        /// </summary>
        [Key]
        [StringLength(12)]
        public string CountryCode { get; set; }

        /// <summary>
        /// The English name of the country
        /// </summary>
        [Required]
        [StringLength(512)]
        public string CountryName { get; set; }

        /// <summary>
        /// The native name of the country
        /// </summary>
        [Required]
        [StringLength(512)]
        public string NativeCountryName { get; set; }
    }
}