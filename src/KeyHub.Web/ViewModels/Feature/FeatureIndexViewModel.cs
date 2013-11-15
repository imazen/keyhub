using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.Feature
{
    /// <summary>
    /// Viewmodel for index list of Features
    /// </summary>
    public class FeatureIndexViewModel : RedirectUrlModel
    {
        public FeatureIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor entity</param>
        public FeatureIndexViewModel(List<Model.Feature> featureList)
            : base()
        {
            Features = new List<FeatureIndexViewItem>();
            foreach (Model.Feature entity in featureList)
            {
                Features.Add(new FeatureIndexViewItem(entity, entity.Vendor));
            }
        }

        /// <summary>
        /// List of features
        /// </summary>
        public List<FeatureIndexViewItem> Features { get; set; }
    }

    /// <summary>
    /// FeatureViewModel extension that includes the name of the vendor
    /// </summary>
    public class FeatureIndexViewItem : FeatureViewModel
    {
        public FeatureIndexViewItem(Model.Feature feature, Model.Vendor vendor)
            : base(feature)
        {
            VendorName = vendor.Name;
        }

        /// <summary>
        /// OrganisationName of the vendor.
        /// </summary>
        [DisplayName("Vendor")]
        public string VendorName { get; set; }
    }
}