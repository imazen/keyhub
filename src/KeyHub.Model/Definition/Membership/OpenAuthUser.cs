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
    /// Represents additional OpenAuth information for each user (1 user can have multiple OpenAuth entries)
    /// </summary>
    public class OpenAuthUser
    {
        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(1024)]
        public string OpenAuthIdentifier { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}