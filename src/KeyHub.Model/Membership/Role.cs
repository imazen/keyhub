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
        /// The application this role is associated with
        /// </summary>
        [Required]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Unique role ID
        /// </summary>
        [Key]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Name for this role
        /// </summary>
        [Required]
        [StringLength(256)]
        public string RoleName { get; set; }

        /// <summary>
        /// Description for this role
        /// </summary>
        [StringLength(256)]
        public string Description { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }

        /// <summary>
        /// Gets a list of the users in this role
        /// </summary>
        public virtual ICollection<UserInRole> UsersInRoles { get; set; }
    }
}