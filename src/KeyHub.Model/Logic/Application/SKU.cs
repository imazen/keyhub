using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// SKU logic
    /// </summary>
    public partial class SKU
    {
        /// <summary>
        /// Add SKUFeatures to SKU based on new feature Guids
        /// </summary>
        /// <param name="NewFeatureGuids">List of new feature Guids to add</param>
        public void AddFeatures(IEnumerable<Guid> NewFeatureGuids)
        {
            foreach (Guid newFeatureGuid in NewFeatureGuids)
                this.SkuFeatures.Add(new SkuFeature() { FeatureId = newFeatureGuid, SkuId = this.SkuId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RemovedFeatureGuids"></param>
        public void RemoveFeatures(IEnumerable<Guid> RemovedFeatureGuids)
        {
            foreach (Guid removedFeatureGuid in RemovedFeatureGuids)
            {
                SkuFeature removedSKUFeature = (from f in this.SkuFeatures where f.FeatureId == removedFeatureGuid select f).FirstOrDefault();
                this.SkuFeatures.Remove(removedSKUFeature);
            }
        }
    }
}
