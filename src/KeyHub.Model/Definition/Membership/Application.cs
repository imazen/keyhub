using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KeyHub.Model
{
    /// <summary>
    /// The application class for the Membership provider
    /// </summary>
    public partial class Application
    {
        /// <summary>
        /// Name for this application
        /// </summary>
        [Required]
        [StringLength(235)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// The unique key for this application
        /// </summary>
        [Key]
        public System.Guid ApplicationId { get; set; }

        /// <summary>
        /// Short description for this application
        /// </summary>
        [StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// All the members associated with this application
        /// </summary>
        public virtual ICollection<Membership> Memberships { get; set; }

        /// <summary>
        /// All the roles associated with this application
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        /// <summary>
        /// All the users associated with this application
        /// </summary>
        public virtual ICollection<User> Users { get; set; }

        /// <summary>
        /// Constructs a new Application class
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Application()
        {
            Memberships = new List<Membership>();
            Roles = new List<Role>();
            Users = new List<User>();
        }
    }
}