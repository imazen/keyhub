using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.Feature
{
    /// <summary>
    /// Viewmodel for a single feature 
    /// </summary>
    public class FeatureViewModel : RedirectUrlModel
    {
        public FeatureViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="feature">Feature that this viewmodel represents</param>
        public FeatureViewModel(Model.Feature feature)
            : this()
        {
            FeatureId = feature.FeatureId;
            VendorId = feature.VendorId;
            FeatureCode = feature.FeatureCode;
            FeatureName = feature.FeatureName;
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid FeatureId { get; set; }

        /// <summary>
        /// The vendor (owner) of this feature
        /// </summary>
        [Required]
        [DisplayName("Vendor")]
        public Guid VendorId { get; set; }

        /// <summary>
        /// The unique code for this feature
        /// </summary>
        [Required]
        public Guid FeatureCode { get; set; }

        /// <summary>
        /// The name for this feature
        /// </summary>
        [Required]
        public String FeatureName { get; set; }
    }
}