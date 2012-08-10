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
    /// Provides registration of customer application (the developers who use it)
    /// </summary>
    public class CustomerAppKey
    {
        /// <summary>
        /// Indentifier for the CustomerAppKey entity
        /// </summary>
        [Key]
        public int CustomerAppKeyId { get; set; }

        /// <summary>
        /// Indentifier for the CustomerApp entity
        /// </summary>
        [Required]
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// The <see cref="CustomerApp"/> this key is associated with
        /// </summary>
        [ForeignKey("CustomerAppId")]
        public CustomerApp CustomerApp { get; set; }

        /// <summary>
        /// Unique application key this <see cref="CustomerApp"/> is generated with.
        /// </summary>
        [Required]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid AppKey { get; set; }
    }
}