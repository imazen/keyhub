using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class FeatureTestData
    {
        public static Feature Create(Guid featureCode)
        {
            return new Feature
            {
                FeatureId = Guid.NewGuid(),
                FeatureCode = featureCode
            };
        }
    }
}
