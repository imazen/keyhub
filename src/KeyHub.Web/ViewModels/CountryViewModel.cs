using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels
{
    public class CountryViewModel
    {
        /// <summary>
        /// Unique Country code
        /// </summary>
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