using KeyHub.Data;
using KeyHub.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Validates licenses
    /// </summary>
    public static class LicenseValidator
    {
        /// <summary>
        /// Validate licenses
        /// </summary>
        /// <param name="appKey">AppKey from CustomerAppKeys table</param>
        /// <param name="domainValidations"></param>
        /// <returns></returns>
        public static IEnumerable<DomainValidationResult> ValidateLicense(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            using (var context = new DataContext())
            {
                CustomerApp customerApp = context.CustomerAppKeys
                    .Where(x => x.AppKey == appKey)
                    .Select(x => x.CustomerApp)
                    .Include(x => x.LicenseCustomerApps)
                    .FirstOrDefault();

                if (customerApp == null)
                {
                    return null;
                }

                IEnumerable<License> licenses = customerApp.LicenseCustomerApps
                    .Select(x => x.License)
                    .ToList();
                var domainValidationResults = new List<DomainValidationResult>();

                foreach (DomainValidation domainValidation in domainValidations)
                {
                    foreach (License license in licenses)
                    {
                        IEnumerable<Guid> featureIds = license.Sku.SkuFeatures.Select(x => x.FeatureId).ToList();
                        List<Guid> featureCodes = context.Features
                            .Where(x => featureIds.Contains(x.FeatureId))
                            .Select(x => x.FeatureCode)
                            .ToList();

                        var domainValidationResult = new DomainValidationResult
                        {
                            DomainName = domainValidation.DomainName,
                            OwnerName = license.OwnerName,
                            Issued = license.LicenseIssued,
                            Expires = license.LicenseExpires,
                            Features = featureCodes
                        };

                        domainValidationResults.Add(domainValidationResult);
                    }
                }

                return domainValidationResults;
            }
        }
    }
}
