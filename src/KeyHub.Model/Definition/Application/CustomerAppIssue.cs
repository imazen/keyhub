using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    public class CustomerAppIssue
    {
        /// <summary>
        /// Indentifier for the CustomerAppIssue entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerAppIssueId { get; set; }

        /// <summary>
        /// The customer app this issue belongs to
        /// </summary>
        [Required]
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// DateTime of occurance of the issue
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Severity of the issue
        /// </summary>
        public ApplicationIssueSeverity Severity { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// The customer app this issue belongs to
        /// </summary>
        [ForeignKey("CustomerAppId")]
        public CustomerApp CustomerApp { get; set; }
    }
}
