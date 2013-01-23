using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class CustomerAppKeyTestData
    {
        public static CustomerAppKey Create(Guid appKey)
        {
            return new CustomerAppKey
            {
                AppKey = appKey
            };
        }
    }
}
