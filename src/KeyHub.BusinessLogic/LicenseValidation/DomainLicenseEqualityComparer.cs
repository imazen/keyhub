using KeyHub.Model;
using System.Collections.Generic;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    public class DomainLicenseEqualityComparer : IEqualityComparer<DomainLicense>
    {
        public bool Equals(DomainLicense x, DomainLicense y)
        {
            return x.DomainName == y.DomainName && x.LicenseId == y.LicenseId;
        }

        public int GetHashCode(DomainLicense obj)
        {
            return (obj.DomainName + obj.LicenseId).GetHashCode();
        }
    }
}
