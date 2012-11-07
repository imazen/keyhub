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
    public class TransactionItem : IModelItem
    {
        /// <summary>
        /// Indentifier for the TransactionItem entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
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

        /// <summary>
        /// 
        /// </summary>
        public Guid? LicenseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; }

        /// <summary>
        /// The SKUId this transaction item buys
        /// </summary>
        [Required]
        public Guid SkuId { get; set; }

        /// <summary>
        /// The SKU this transaction item buys
        /// </summary>
        [ForeignKey("SkuId")]
        public SKU Sku { get; set; }
    }
}