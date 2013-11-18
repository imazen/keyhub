using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Feature
{
    /// <summary>
    /// Viewmodel for creation of a Feature
    /// </summary>
    public class FeatureCreateEditViewModel
    {
        public static FeatureCreateEditViewModel ForCreate(IDataContextByUser context)
        {
            FeatureCreateEditViewModel viewModel = new FeatureCreateEditViewModel()
            {
                Feature = new FeatureViewModel(new Model.Feature()),
                VendorList = context.Vendors.ToList().ToSelectList(v => v.ObjectId, v => v.Name)
            };

            return viewModel;
        }

        public static FeatureCreateEditViewModel ForEdit(IDataContextByUser context, Guid key)
        {
            var feature = context.Features.SingleOrDefault(f => f.FeatureId == key);

            if (feature == null)
            {
                return null;
            }
            else
            {
                var viewModel = ForCreate(context);
                viewModel.Feature = new FeatureViewModel(feature);
                return viewModel;
            }
        }

        public void ApplyToEntity(IDataContextByUser dataContext, Model.Feature feature)
        {
            var vendor = dataContext.Vendors.Single(v => v.ObjectId == Feature.VendorId);  // Load the vendor to ensure its authorized

            feature.Vendor = vendor;
            feature.FeatureName = Feature.FeatureName;
            feature.FeatureCode = Feature.FeatureCode;
        }

        /// <summary>
        /// Edited Feature
        /// </summary>
        public FeatureViewModel Feature { get; set; }

        /// <summary>
        /// List of vendors to select
        /// </summary>
        public SelectList VendorList { get; set; }
    }
}