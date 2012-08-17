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
    /// Transaction for storing transaction information
    /// </summary>
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionItems = new List<TransactionItem>();
            Status = TransactionStatus.Create;
        }

        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

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
        /// The list of transaction items this SKU consists of.
        /// </summary>
        public virtual ICollection<TransactionItem> TransactionItems { get; set; }
    }
}