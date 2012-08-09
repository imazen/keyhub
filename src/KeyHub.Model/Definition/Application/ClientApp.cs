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
    public class ClientApp
    {
        /// <summary>
        /// Indentifier for the ClientApp entity
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid ClientAppId { get; set; }

        /// <summary>
        /// The name of the client application
        /// </summary>
        [Required]
        [StringLength(256)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// A list of licenses (<see cref="License"/>) this application is associated with
        /// </summary>
        public virtual ICollection<License> Licenses { get; set; }
    }
}