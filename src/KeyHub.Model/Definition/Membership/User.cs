using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Deines the user for this application
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// The application this user is associated with
        /// </summary>
        [Required]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// USername (loginname) for this user
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Indicated wether this user is anonymous or not
        /// </summary>
        [Required]
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Last activity date
        /// </summary>
        [Required]
        public DateTime LastActivityDate { get; set; }

        [ForeignKey("UserId")]
        public virtual Membership Membership { get; set; }

        [ForeignKey("UserId")]
        public virtual Profile Profile { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }

        public virtual ICollection<UserInRole> UsersInRoles { get; set; }

        public virtual ICollection<OpenAuthUser> OpenAuthUsers { get; set; }
    }
}