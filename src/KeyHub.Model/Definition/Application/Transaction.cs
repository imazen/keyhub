using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    // TODO: Finish this entity
    /// <summary>
    /// Transaction for storing transaction information
    /// </summary>
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionItems = new List<TransactionItem>();
        }

        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        /// <summary>
        /// The list of transaction items this SKU consists of.
        /// </summary>
        public virtual ICollection<TransactionItem> TransactionItems { get; set; }
    }
}