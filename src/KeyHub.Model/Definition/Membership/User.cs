using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the user for this application
    /// </summary>
    public partial class User
    {
        public User()
        {
            UserInRoles = new List<UserInRole>();
            Rights = new List<UserObjectRight>();
        }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// The field used by ASP.NET Membership system to identify users (a guid value).
        /// </summary>
        [Required]
        [StringLength(40)]
        public string MembershipUserIdentifier { get; set; }

        /// <summary>
        /// E-mail address for this user
        /// </summary>
        [StringLength(256)]
        public string Email { get; set; }

        [ForeignKey("UserId")]
        public virtual Membership Membership { get; set; }

        public virtual ICollection<UserInRole> UserInRoles { get; set; }

        public virtual ICollection<UserObjectRight> Rights { get; set; }
    }
}