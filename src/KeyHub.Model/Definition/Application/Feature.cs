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
    /// A Feature is what the .dlls  care about: ex: both Performance Enterprise and Performance Professional
    /// would use the same sets of feature codes.
    ///
    /// Each plugin would typically get its own feature code. Eg. performance would include 3
    /// feature codes right now: DiskCache, PrettyGifs, and AnimatedGifs.
    /// </summary>
    public class Feature : IModelItem
    {
        public Feature()
        {
            SkuFeatures = new List<SkuFeature>();
            FeatureCode = Guid.NewGuid();
        }

        /// <summary>
        /// Indentifier for the Feature entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FeatureId { get; set; }

        /// <summary>
        /// The vendor (owner) of this feature
        /// </summary>
        [Required]
        public Guid VendorId { get; set; }

        /// <summary>
        /// The vendor (owner) of this feature
        /// </summary>
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }

        /// <summary>
        /// The unique code for this feature
        /// </summary>
        [Required]
        public Guid FeatureCode { get; set; }

        /// <summary>
        /// The name for this feature
        /// </summary>
        [Required]
        [StringLength(256)]
        [Column(TypeName = "varchar")]
        public string FeatureName { get; set; }

        /// <summary>
        /// The list of SKU's this feature is part of
        /// </summary>
        public virtual ICollection<SkuFeature> SkuFeatures { get; set; }
    }
}