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
    /// Provides the join table between the <see cref="License"/> and <see cref="ClientApp"/> entities
    /// </summary>
    public class LicenseClientApp
    {
        /// <summary>
        /// Unqiue LicenseId
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Unique ClientAppId
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public Guid ClientAppId { get; set; }

        /// <summary>
        /// Relation to <see cref="License"/>
        /// </summary>
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; }

        /// <summary>
        /// Relation to <see cref="ClientApp"/>
        /// </summary>
        [ForeignKey("ClientAppId")]
        public virtual ClientApp ClientApp { get; set; }
    }
}