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
            SKU = new SKUViewModel(sku);
            VendorList = vendors.ToSelectList(x => x.ObjectId, x => x.OrganisationName);
            PrivateKeyList = privateKeys.ToSelectList(x => x.PrivateKeyId, x => x.DisplayName);
            FeatureList = features.ToSelectList(x => x.FeatureId, x => x.FeatureCode);
        }

        /// <summary>
        /// Edited vendor
        /// </summary>
        public SKUViewModel SKU { get; set; }

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
        public SelectList FeatureList { get; set; }

        /// <summary>
        /// Convert back to SKU instance
        /// </summary>
        /// <param name="original">Original SKU. If Null a new instance is created.</param>
        /// <returns>SKU containing viewmodel data </returns>
        public override Model.SKU ToEntity(Model.SKU original)
        {
            return SKU.ToEntity(original);
        }
    }
}