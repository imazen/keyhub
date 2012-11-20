using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines an application role
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// Unique role ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        /// <summary>
        /// Name for this role
        /// </summary>
        [Required]
        [StringLength(256)]
        public string RoleName { get; set; }

        /// <summary>
        /// Gets a list of the users in this role
        /// </summary>
        public virtual ICollection<UserInRole> UsersInRoles { get; set; }
    }
}