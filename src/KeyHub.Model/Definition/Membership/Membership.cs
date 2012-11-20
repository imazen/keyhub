using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// The Membership class for the Membership provider
    /// </summary>
    public partial class Membership
    {
        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// The date this user was created
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// The ConfirmationToken
        /// </summary>
        [StringLength(128)]
        public string ConfirmationToken { get; set; }

        /// <summary>
        /// Wether the user is confirmed or not
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// The date this user last failed to log in
        /// </summary>
        public DateTime? LastPasswordFailureDate { get; set; }

        /// <summary>
        /// The amount of failures since last successfull login
        /// </summary>
        [Required]
        public int PasswordFailuresSinceLastSuccess { get; set; }

        /// <summary>
        /// Password for this user
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        /// <summary>
        /// The date when this user changed his password
        /// </summary>
        public DateTime? PasswordChangedDate { get; set; }

        /// <summary>
        /// Password salt value
        /// </summary>
        [Required]
        [StringLength(128)]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Verification token of the password
        /// </summary>
        [StringLength(128)]
        public string PasswordVerificationToken { get; set; }

        /// <summary>
        /// Expiration date of the verification token
        /// </summary>
        public DateTime? PasswordVerificationTokenExirationDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}