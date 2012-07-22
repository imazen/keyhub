using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Defnies the profile for a specific user
    /// </summary>
    public partial class Profile
    {
        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// All the property names for this user
        /// </summary>
        [Required]
        [StringLength(4000)]
        public string PropertyNames { get; set; }

        /// <summary>
        /// All the property values for this user
        /// </summary>
        [Required]
        [StringLength(4000)]
        public string PropertyValueStrings { get; set; }

        /// <summary>
        /// The property values in Binary
        /// </summary>
        [Required]
        public byte[] PropertyValueBinary { get; set; }

        /// <summary>
        /// The date the properties where updates for the last time
        /// </summary>
        [Required]
        public DateTime LastUpdatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}