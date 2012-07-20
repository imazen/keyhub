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
        /// The application ID this membership entry is asociated with
        /// </summary>
        [Required]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// Password for this user
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        /// <summary>
        /// The format of this password
        /// </summary>
        [Required]
        public int PasswordFormat { get; set; }

        /// <summary>
        /// Password salt value
        /// </summary>
        [Required]
        [StringLength(128)]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// E-mail address for this user
        /// </summary>
        [StringLength(256)]
        public string Email { get; set; }

        /// <summary>
        /// Secret password question
        /// </summary>
        [StringLength(256)]
        public string PasswordQuestion { get; set; }

        /// <summary>
        /// Anwser to the secret question
        /// </summary>
        [StringLength(128)]
        public string PasswordAnswer { get; set; }

        /// <summary>
        /// Wether the user is aproved or not
        /// </summary>
        [Required]
        public bool IsApproved { get; set; }

        /// <summary>
        /// Wether the user is locked out or not
        /// </summary>
        [Required]
        public bool IsLockedOut { get; set; }

        /// <summary>
        /// The date this user was created
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The date this user has last logged in succesfully
        /// </summary>
        [Required]
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// The date when this user changed his password
        /// </summary>
        [Required]
        public DateTime LastPasswordChangedDate { get; set; }

        /// <summary>
        /// The date this user was last locked out
        /// </summary>
        [Required]
        public DateTime LastLockoutDate { get; set; }

        /// <summary>
        /// Indicated how many times this user has typed his password wrong
        /// </summary>
        [Required]
        public int FailedPasswordAttemptCount { get; set; }

        /// <summary>
        /// The date when this user started to wrongly input the password
        /// </summary>
        [Required]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        /// <summary>
        /// Indicated how many times this user has failed to anwser his secret question
        /// </summary>
        [Required]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        /// <summary>
        /// The date when this user started to anwser his secret question wrong
        /// </summary>
        [Required]
        public DateTime FailedPasswordAnswerAttemptWindowsStart { get; set; }

        /// <summary>
        /// Internal comment for this user
        /// </summary>
        [StringLength(256)]
        public string Comment { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}