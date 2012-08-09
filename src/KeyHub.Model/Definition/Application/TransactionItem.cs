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
    /// Transaction row for storing transaction information
    /// </summary>
    public class TransactionItem
    {
        /// <summary>
        /// Indentifier for the TransactionItem entity.
        /// </summary>
        [Key]
        public int TransactionItemId { get; set; }

        /// <summary>
        /// The transaction main records (parent of this row)
        /// </summary>
        [Required]
        public int TransactionId { get; set; }

        /// <summary>
        /// The transaction main records (parent of this row)
        /// </summary>
        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }
    }
}