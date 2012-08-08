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
    public class FeatureViewModel : BaseViewModel<Model.Feature>
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
        }

        /// <summary>
        /// Convert back to Feature instance
        /// </summary>
        /// <param name="original">Original Feature. If Null a new instance is created.</param>
        /// <returns>Feature containing viewmodel data </returns>
        public override Model.Feature ToEntity(Model.Feature original)
        {
            Model.Feature current = original ?? new Model.Feature();

            current.VendorId = this.VendorId;
            current.FeatureCode = this.FeatureCode;

            return current;
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
        [StringLength(256)]
        public string FeatureCode { get; set; }
    }
}