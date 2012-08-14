using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.SKU
{
    /// <summary>
    /// Viewmodel for index list of SKUs
    /// </summary>
    public class SKUIndexViewModel : BaseViewModel<Model.SKU>
    {
        public SKUIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="skuList">List of SKU entities</param>
        public SKUIndexViewModel(List<Model.SKU> skuList)
            : this()
        {
            SKUs = new List<SKUIndexViewItem>();
            foreach (Model.SKU entity in skuList)
            {
                SKUs.Add(new SKUIndexViewItem(entity, entity.Vendor, entity.PrivateKey, 
                    entity.SkuFeatures.OrderBy(x => x.Feature.FeatureCode)));
            }
        }

        /// <summary>
        /// List of SKUs
        /// </summary>
        public List<SKUIndexViewItem> SKUs { get; set; }

        /// <summary>
        /// Convert back to SKU instance
        /// </summary>
        /// <param name="original">Original SKU. If Null a new instance is created.</param>
        /// <returns>SKU containing viewmodel data </returns>
        public override Model.SKU ToEntity(Model.SKU original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// SKUViewModel extension that includes Vendor name, Primary key name, summary of features
    /// </summary>
    public class SKUIndexViewItem : SKUViewModel
    {
        public SKUIndexViewItem(Model.SKU sku, Model.Vendor vendor, Model.PrivateKey privateKey, IEnumerable<Model.SkuFeature> skuFeatures)
            : base(sku)
        {
            VendorName = vendor.Name;
            PrivateKeyName = privateKey.DisplayName;
            FeatureSummary = BuildFeatureSummary(skuFeatures);
        }

        /// <summary>
        /// Builds a summary of features assigned to an SKU
        /// </summary>
        /// <param name="skuFeatures">List of assigned features</param>
        /// <returns>Summary</returns>
        private string BuildFeatureSummary(IEnumerable<Model.SkuFeature> skuFeatures)
        {
            const int MAXLINES = 3;
            const string LINEFEED = ", ";
            string summary = "";

            var filteredFeatures = skuFeatures.Take(MAXLINES);

            if (filteredFeatures.Count() > 0)
            {
                summary = string.Join(LINEFEED, filteredFeatures.Select(x => x.Feature.FeatureCode));
                if (skuFeatures.Count() > MAXLINES)
                    summary += string.Format(" and {0} more...", skuFeatures.Count() - MAXLINES);
            }
            else
                summary = "No features assigned";

            return summary;
        }

        /// <summary>
        /// OrganisationName of the Vendor
        /// </summary>
        [DisplayName("Vendor")]
        public string VendorName { get; set; }

        /// <summary>
        /// DisplayName of the Private key
        /// </summary>
        [DisplayName("Private key")]
        public string PrivateKeyName { get; set; }

        /// <summary>
        /// Summary of assigned Features
        /// </summary>
        [DisplayName("Features")]
        public string FeatureSummary { get; set; }
    }
}