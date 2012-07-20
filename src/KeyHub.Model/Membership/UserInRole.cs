using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the link between users and roles
    /// </summary>
    public class UserInRole
    {
        /// <summary>
        /// Unqiue User ID
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Unique Role ID
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public Guid RoleId { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}