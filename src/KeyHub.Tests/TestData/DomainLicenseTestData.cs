using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class DomainLicenseTestData
    {
        public static DomainLicense Create(string domainName, License license)
        {
            return new DomainLicense
            {
                DomainName = domainName,
                License = license,
                LicenseId = license.ObjectId
            };
        }
    }
}
