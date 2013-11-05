using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class SkuTestData
    {
        public static SKU Create(PrivateKey privateKey, params Feature[] features)
        {
            SKU sku = new SKU
            {
                SkuId = Guid.NewGuid(),
                SkuFeatures = new List<SkuFeature>(),
                PrivateKey = privateKey,
                PrivateKeyId = privateKey.PrivateKeyId
            };

            foreach (var feature in features)
            {
                sku.SkuFeatures.Add(new SkuFeature
                {
                    Feature = feature,
                    FeatureId = feature.FeatureId,
                    Sku = sku,
                    SkuId = sku.SkuId
                });
            }

            return sku;
        }
    }
}
