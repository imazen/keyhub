using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Feature
{
    /// <summary>
    /// Viewmodel for editing of a Feature
    /// </summary>
    /// <summary>
    /// Viewmodel for editing a Vendor
    /// </summary>
    public class FeatureEditViewModel : BaseViewModel<Model.Feature>
    {
        public FeatureEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="feature">Feature entity</param>
        /// <param name="vendors">Vendor query for selectionlist</param>
        public FeatureEditViewModel(Model.Feature feature, List<Model.Vendor> vendors)
        {
            Feature = new FeatureViewModel(feature);
            VendorList = vendors.ToSelectList(v => v.ObjectId, v => v.Name);
        }

        /// <summary>
        /// Edited vendor
        /// </summary>
        public FeatureViewModel Feature { get; set; }

        /// <summary>
        /// List of countries to select
        /// </summary>
        public SelectList VendorList { get; set; }

        /// <summary>
        /// Convert back to Feature instance
        /// </summary>
        /// <param name="original">Original Feature. If Null a new instance is created.</param>
        /// <returns>Feature containing viewmodel data </returns>
        public override Model.Feature ToEntity(Model.Feature original)
        {
            return Feature.ToEntity(original);
        }
    }
}