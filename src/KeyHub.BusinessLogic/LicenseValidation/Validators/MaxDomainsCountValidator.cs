using KeyHub.Core.Logging;
using KeyHub.Model;
using KeyHub.Runtime;
using System;
using System.Collections.Generic;

namespace KeyHub.BusinessLogic.LicenseValidation.Validators
{
    /// <summary>
    /// Maximum domains count validation
    /// </summary>
    public class MaxDomainsCountValidator : IValidator
    {
        /// <summary>
        /// Validate domain licenses
        /// </summary>
        /// <param name="domainLicenses"></param>
        /// <returns></returns>
        public List<DomainLicense> Validate(IEnumerable<DomainLicense> domainLicenses)
        {
            Dictionary<Guid, int> licenseIdToUsedDomainsCount = new Dictionary<Guid, int>();
            List<DomainLicense> result = new List<DomainLicense>();

            foreach (DomainLicense domainLicense in domainLicenses)
            {
                if (domainLicense.License.Sku.MaxDomains.HasValue)
                {
                    int usedDomainsCount;
                    int maxDomains = domainLicense.License.Sku.MaxDomains.Value;

                    if (!licenseIdToUsedDomainsCount.TryGetValue(domainLicense.LicenseId, out usedDomainsCount))
                    {
                        usedDomainsCount = 0;
                    }

                    if (usedDomainsCount < maxDomains)
                    {
                        result.Add(domainLicense);

                        licenseIdToUsedDomainsCount[domainLicense.LicenseId] = usedDomainsCount + 1;
                    }
                    else
                    {
                        // need to notify
                        LogContext.Instance.Log(string.Format("MaxDomains violation: domain: {0}, licenseId: {1}", domainLicense.DomainName, domainLicense.LicenseId), LogTypes.Info);
                    }
                }
                else
                {
                    result.Add(domainLicense);
                }
            }

            return result;
        }
    }
}
