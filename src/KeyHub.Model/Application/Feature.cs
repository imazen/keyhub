using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class Feature
    {
        /// <summary>
        /// Indentifier for the Feature entity
        /// </summary>
        [Key]
        public Guid FeatureId { get; set; }

        /// <summary>
        /// The vendor (owner) of this feature
        /// </summary>
        [Required]
        public virtual Vendor Vendor { get; set; }

        /// <summary>
        /// The unique code for this feature
        /// </summary>
        [Required]
        [StringLength(256)]
        public string FeatureCode { get; set; }

        /// <summary>
        /// The list of SKU's this feature is part of
        /// </summary>
        public virtual ICollection<SkuFeature> SkuFeatures { get; set; }
    }
}