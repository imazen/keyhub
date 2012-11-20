using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// The OAuthMembership class for the Membership provider
    /// </summary>
    public partial class OAuthMembership
    {
        /// <summary>
        /// Provider
        /// </summary>
        [Key]
        [Required]
        [StringLength(30)]
        [Column(Order = 1)]
        public string Provider { get; set; }

        /// <summary>
        /// Provider user ID
        /// </summary>
        [Key]
        [Required]
        [StringLength(100)]
        [Column(Order = 2)]
        public string ProviderUserId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }
}