using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class LicenseTestData
    {
        public static License Create(SKU sku)
        {
            return new License
            {
                Sku = sku,
                SkuId = sku.SkuId
            };
        }
    }
}
