using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// The domains registered with the associated license. 
    /// The domains can be automaticly created, and/or manually created/managaed through the portal login
    /// 
    /// For simplicity, we do not differentiate between domains and subdomains. 
    /// In the .dll, we automatically strip "www." off domains, but other subdomains will require separate licenses.
    /// </summary>
    public partial class DomainLicense : IModelItem
    {
        /// <summary>
        /// Unique LicenseDomainId as identity for a domain
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DomainLicenseId { get; set; }

        /// <summary>
        /// The license this domains is associated with
        /// </summary>
        [Required]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// The license this domains is associated with
        /// </summary>
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; }

        /// <summary>
        /// The domainname for this LicenseDomainRecord
        /// </summary>
        [Required]
        [StringLength(256)]
        [Column(TypeName = "varchar")]
        public string DomainName { get; set; }

        /// <summary>
        /// The date this domain has been created (either automaticly or manual)
        /// Manually created licenses may or may not have an expiration date. 
        /// If there is no expiration, it should be null.
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DomainLicenseIssued { get; set; }

        /// <summary>
        /// The date this domain license will expire
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? DomainLicenseExpires { get; set; }

        /// <summary>
        /// Should be true if automatically generated
        /// </summary>
        [Required]
        public bool AutomaticlyCreated { get; set; }

        /// <summary>
        /// The private key in bytes, encrypted with KeyHub.Common.SymmetricEncryption
        /// </summary>
        [Required]
        [MaxLength(4096)]
        public byte[] KeyBytes { get; set; }
    }
}
