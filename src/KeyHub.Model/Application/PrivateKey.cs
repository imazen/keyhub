using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Private key entity for encryption/decryption purposes of license keys.
    /// </summary>
    public class PrivateKey
    {
        /// <summary>
        /// Indentifier for the PrivateKey entity.
        /// </summary>
        [Key]
        public int PrivateKeyId { get; set; }

        /// <summary>
        /// The vendor this key is owned by.
        /// </summary>
        [Required]
        public virtual Vendor Vendor { get; set; }

        /// <summary>
        /// Display name for this private key
        /// </summary>
        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }

        /// <summary>
        /// The private key in bytes
        /// </summary>
        [Required]
        [MaxLength(4096)]
        public byte[] KeyBytes { get; set; }

        /// <summary>
        /// Gets the SKU's that use this private key
        /// </summary>
        public virtual ICollection<SKU> SKUs { get; set; }
    }
}