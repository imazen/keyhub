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
    /// Transaction row for storing transaction information
    /// </summary>
    public class TransactionIgnoredItem : IModelItem
    {
        /// <summary>
        /// Indentifier for the TransactionItem entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionItemId { get; set; }

        /// <summary>
        /// The transaction main records (parent of this row)
        /// </summary>
        [Required]
        public Guid TransactionId { get; set; }

        /// <summary>
        /// The transaction main records (parent of this row)
        /// </summary>
        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }

        /// <summary>
        /// The SKUId this transaction item buys
        /// </summary>
        [Required]
        [StringLength(1024)]
        public string Description { get; set; }
    }
}