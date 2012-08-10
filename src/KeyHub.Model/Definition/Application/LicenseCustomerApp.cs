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
    /// Provides the join table between the <see cref="License"/> and <see cref="CustomerApp"/> entities
    /// </summary>
    public class LicenseCustomerApp
    {
        /// <summary>
        /// Unqiue LicenseId
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Unique CustomerAppId
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// Relation to <see cref="License"/>
        /// </summary>
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; }

        /// <summary>
        /// Relation to <see cref="CustomerApp"/>
        /// </summary>
        [ForeignKey("CustomerAppId")]
        public virtual CustomerApp CustomerApp { get; set; }
    }
}