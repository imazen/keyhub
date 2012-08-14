using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.SKU
{
    /// <summary>
    /// Viewmodel for editing an SKU
    /// </summary>
    public class SKUEditViewModel : BaseViewModel<Model.SKU>
    {
        public SKUEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="sku">SKU entity</param>
        /// <param name="vendors">List of vendors to select</param>
        /// <param name="privateKeys">List of privateKeys to select</param>
        /// <param name="features">List of features to select</param>
        public SKUEditViewModel(Model.SKU sku, List<Model.Vendor> vendors, List<Model.PrivateKey> privateKeys,
            List<Model.Feature> features)
        {
            SKU = new SKUEditViewItem(sku);

            VendorList = vendors.ToSelectList(x => x.ObjectId, x => x.Name);
            PrivateKeyList = privateKeys.ToSelectList(x => x.PrivateKeyId, x => x.DisplayName);
            FeatureList = features.ToMultiSelectList(x => x.FeatureId, x => x.FeatureCode);
        }

        /// <summary>
        /// Edited vendor
        /// </summary>
        public SKUEditViewItem SKU { get; set; }

        /// <summary>
        /// List of vendors to select
        /// </summary>
        public SelectList VendorList { get; set; }

        /// <summary>
        /// List of private keys to select
        /// </summary>
        public SelectList PrivateKeyList { get; set; }

        /// <summary>
        /// List of features keys to select
        /// </summary>
        public MultiSelectList FeatureList { get; set; }

        /// <summary>
        /// Convert back to SKU instance
        /// </summary>
        /// <param name="original">Original SKU. If Null a new instance is created.</param>
        /// <returns>SKU containing viewmodel data </returns>
        public override Model.SKU ToEntity(Model.SKU original)
        {
            return SKU.ToEntity(original);
        }

        /// <summary>
        /// Retrieve al list of new features for this SKU
        /// </summary>
        /// <param name="original">Original SKU</param>
        /// <returns>List of new feature Guids</returns>
        public List<Guid> GetNewFeatureGUIDs(Model.SKU original)
        {
            var originalFeatures = (from x in original.SkuFeatures select x.FeatureId);
            return SKU.SelectedFeatureGUIDs.Except(originalFeatures).ToList();
        }

        /// <summary>
        /// Retrieve a list of removed features for this SKU
        /// </summary>
        /// <param name="original">Original SKU</param>
        /// <returns>List of removed feature Guids</returns>
        public List<Guid> GetRemovedFeatureGUIDs(Model.SKU original)
        {
            var originalFeatures = (from x in original.SkuFeatures select x.FeatureId);
            return originalFeatures.Except(SKU.SelectedFeatureGUIDs).ToList();
        }
    }

    /// <summary>
    /// SKUViewModel extension that contains a list of SelectedFeature Guids
    /// </summary>
    public class SKUEditViewItem : SKUViewModel
    {
        public SKUEditViewItem() : base() 
        {
            new List<Guid>();
        }

        public SKUEditViewItem(Model.SKU sku)
            : base(sku)
        {
            SelectedFeatureGUIDs = new List<Guid>(
                    sku.SkuFeatures.Select(x => x.FeatureId)
                );
        }

        /// <summary>
        /// Guid list of assigned features
        /// </summary>
        public List<Guid> SelectedFeatureGUIDs { get; set; }
    }
}