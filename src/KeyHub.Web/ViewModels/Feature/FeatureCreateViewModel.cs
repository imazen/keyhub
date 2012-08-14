using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.Feature
{
    /// <summary>
    /// Viewmodel for creation of a Feature
    /// </summary>
    public class FeatureCreateViewModel : BaseViewModel<Model.Feature>
    {
        public FeatureCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendors">Vendors for selectionlist</param>
        public FeatureCreateViewModel(List<Model.Vendor> vendors)
        {
            Feature = new FeatureViewModel(new Model.Feature());
            VendorList = vendors.ToSelectList(v => v.ObjectId, v => v.Name);
        }

        /// <summary>
        /// Edited Feature
        /// </summary>
        public FeatureViewModel Feature { get; set; }

        /// <summary>
        /// List of vendors to select
        /// </summary>
        public SelectList VendorList { get; set; }

        /// <summary>
        /// Convert back to Feature instance
        /// </summary>
        /// <returns>New Feature containing viewmodel data </returns>
        public override Model.Feature ToEntity(Model.Feature original)
        {
            return Feature.ToEntity(null);
        }
    }
}