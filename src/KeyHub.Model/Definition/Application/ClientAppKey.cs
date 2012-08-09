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
    /// Provides registration of client application (the developers who use it)
    /// </summary>
    public class ClientAppKey
    {
        /// <summary>
        /// Indentifier for the Feature entity
        /// </summary>
        [Key]
        public int ClientAppKeyId { get; set; }

        /// <summary>
        /// Indentifier for the ClientApp entity
        /// </summary>
        [Required]
        public Guid ClientAppId { get; set; }

        /// <summary>
        /// The <see cref="ClientApp"/> this key is associated with
        /// </summary>
        [ForeignKey("ClientAppId")]
        public ClientApp ClientApp { get; set; }

        /// <summary>
        /// Unique application key this <see cref="ClientApp"/> is generated with.
        /// </summary>
        [Required]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid AppKey { get; set; }
    }
}