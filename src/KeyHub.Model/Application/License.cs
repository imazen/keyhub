using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Represents a purchased SKU for storing owner and license information
    /// </summary>
    public class License
    {
        /// <summary>
        /// Indentifier for the License entity
        /// </summary>
        [Key]
        public int LicenseId { get; set; }

        /// <summary>
        /// The SKU bought with this license
        /// </summary>
        [Required]
        public virtual SKU Sku { get; set; }
    }
}