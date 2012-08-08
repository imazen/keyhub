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
    /// Provides the join table between SKU's and Features
    /// </summary>
    public class SkuFeature
    {
        /// <summary>
        /// Unqiue SkuId
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid SkuId { get; set; }

        /// <summary>
        /// Unique FeatureId
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public Guid FeatureId { get; set; }

        /// <summary>
        /// Relation to SKU
        /// </summary>
        [ForeignKey("SkuId")]
        public virtual SKU Sku { get; set; }

        /// <summary>
        /// Relation to Feature
        /// </summary>
        [ForeignKey("FeatureId")]
        public Feature Feature { get; set; }
    }
}