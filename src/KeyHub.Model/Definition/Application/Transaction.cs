using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Transaction for storing transaction information
    /// </summary>
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionItems = new List<TransactionItem>();
            IgnoredItems = new List<TransactionIgnoredItem>();
            Status = TransactionStatus.Create;
        }

        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [Required]
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Date the transaction was created on
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// String transactionmessage
        /// </summary>
        [MaxLength(Int32.MaxValue)]
        public string OriginalRequest { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string PurchaserName { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string PurchaserEmail { get; set; }

        /// <summary>
        /// The list of transaction items this SKU consists of.
        /// </summary>
        public virtual ICollection<TransactionItem> TransactionItems { get; set; }

        /// <summary>
        /// The list of transaction ignored items.
        /// </summary>
        public virtual ICollection<TransactionIgnoredItem> IgnoredItems { get; set; }
    }
}