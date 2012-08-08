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
    public class FeatureIndexViewModel : BaseViewModel<Model.Feature>
    {
        public FeatureIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor entity</param>
        /// <param name="countries">Country query for selectionlist</param>
        public FeatureIndexViewModel(List<Model.Feature> featureList)
            : this()
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

        /// <summary>
        /// Convert back to Feature instance
        /// </summary>
        /// <param name="original">Original Feature. If Null a new instance is created.</param>
        /// <returns>Feature containing viewmodel data </returns>
        public override Model.Feature ToEntity(Model.Feature original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// FeatureViewModel extension that includes the name of the vendor
    /// </summary>
    public class FeatureIndexViewItem : FeatureViewModel
    {
        public FeatureIndexViewItem(Model.Feature feature, Model.Vendor vendor)
            : base(feature)
        {
            VendorName = vendor.OrganisationName;
        }

        /// <summary>
        /// OrganisationName of the vendor.
        /// </summary>
        [DisplayName("Vendor")]
        public string VendorName { get; set; }
    }
}