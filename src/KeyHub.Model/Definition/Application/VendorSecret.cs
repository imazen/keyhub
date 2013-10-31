﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Shared secret used by vendors when POST'ing transactions
    /// </summary>
    public class VendorSecret
    {
        /// <summary>
        /// Indentifier for the PrivateKey entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid VendorSecretId { get; set; }

        /// <summary>
        /// The vendor this key is owned by.
        /// </summary>
        [Required]
        public Guid VendorId { get; set; }

        /// <summary>
        /// The vendor this key is owned by.
        /// </summary>
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }


        /// <summary>
        /// The name of the shared secret (managed by the vendor)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The shared secret, encrypted by SymmetricEncryption & appSetting[DatabaseEncryptionKey]
        /// </summary>
        [Required]
        public string SharedSecret { get; set; }
    }
}
