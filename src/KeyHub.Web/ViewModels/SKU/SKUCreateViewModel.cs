using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.SKU
{
    /// <summary>
    /// Viewmodel for creating an SKU
    /// </summary>
    public class SKUCreateViewModel : BaseViewModel<Model.SKU>
    {
        public SKUCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendors">List of vendors to select</param>
        /// <param name="privateKeys">List of privateKeys to select</param>
        /// <param name="features">List of features to select</param>
        public SKUCreateViewModel(List<Model.Vendor> vendors, List<Model.PrivateKey> privateKeys,
            List<Model.Feature> features)
        {
            SKU = new SKUCreateViewItem(new Model.SKU());

            VendorList = vendors.ToSelectList(x => x.ObjectId, x => x.Name);
            PrivateKeyList = privateKeys.ToSelectList(x => x.PrivateKeyId, x => x.DisplayName);
            FeatureList = features.ToMultiSelectList(x => x.FeatureId, x => x.FeatureCode);
        }

        /// <summary>
        /// Creating SKU
        /// </summary>
        public SKUCreateViewItem SKU { get; set; }

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
            return SKU.ToEntity(null);
        }

        /// <summary>
        /// Retrieve a list of feature Guids
        /// </summary>
        /// <returns>List of assigned feature Guids</returns>
        public List<Guid> GetNewFeatureGUIDs()
        {
            return SKU.SelectedFeatureGUIDs.ToList();
        }
    }

    /// <summary>
    /// SKUViewModel extension that contains a list of SelectedFeature Guids
    /// </summary>
    public class SKUCreateViewItem : SKUViewModel
    {
        public SKUCreateViewItem() : base() 
        {
            SelectedFeatureGUIDs = new List<Guid>();
        }

        public SKUCreateViewItem(Model.SKU sku)
            : base(sku)
        { }

        /// <summary>
        /// Guid list of assigned features
        /// </summary>
        public List<Guid> SelectedFeatureGUIDs { get; set; }
    }
}