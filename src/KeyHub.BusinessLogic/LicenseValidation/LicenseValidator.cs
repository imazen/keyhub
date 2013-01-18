using System.Linq.Expressions;
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
    public class LicenseValidator : IDisposable
    {
        private readonly DataContext context;

        private LicenseValidator()
        {
            context = new DataContext();
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        private IEnumerable<DomainValidationResult> Validate(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            List<DomainLicense> domainLicenses = MatchDomainLicenses(appKey, domainValidations);

            SaveDomainLicenses(domainLicenses);

            return ToDomainVelidationResults(domainLicenses);
        }

        private List<DomainLicense> MatchDomainLicenses(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            CustomerApp matchedCustomerApp = context.CustomerAppKeys
                .Where(x => x.AppKey == appKey)
                .Select(x => x.CustomerApp)
                .Include(x => x.LicenseCustomerApps)
                .FirstOrDefault();

            if (matchedCustomerApp == null)
            {
                // need to notify
                return null;
            }

            IEnumerable<Guid> matchedLicenseIds = matchedCustomerApp.LicenseCustomerApps
                .Select(x => x.License.ObjectId)
                .ToList();

            var domainLicenses = new List<DomainLicense>();

            foreach (DomainValidation domainValidation in domainValidations)
            {
                string domainName = domainValidation.DomainName;
                Guid featureCode = domainValidation.FeatureCode;

                var domainLicense = context.DomainLicenses
                    .Include(x => x.License.Sku.SkuFeatures.Select(s => s.Feature))
                    .FirstOrDefault(x => x.DomainName == domainName
                        && matchedLicenseIds.Contains(x.LicenseId) 
                        && x.License.Sku.SkuFeatures.Select(s => s.Feature.FeatureCode).Contains(featureCode));

                if (domainLicense == null)
                {
                    var featureLicense = context.Licenses
                        .Include(x => x.Sku.SkuFeatures.Select(s => s.Feature))
                        .FirstOrDefault(x => x.Sku.SkuFeatures.Select(s => s.Feature.FeatureCode).Any(y => y == featureCode));

                    if (featureLicense == null)
                    {
                        // we do not have license on a feature or we just do not have this one
                        // need to notify
                        continue;
                    }

                    domainLicense = new DomainLicense
                    {
                        DomainName = domainName,
                        AutomaticlyCreated = true,
                        DomainLicenseIssued = featureLicense.LicenseIssued,
                        DomainLicenseExpires = featureLicense.LicenseExpires,
                        KeyBytes = featureLicense.Sku.PrivateKey.KeyBytes,
                        License = featureLicense,
                        LicenseId = featureLicense.ObjectId
                    };
                }

                domainLicenses.Add(domainLicense);
            }

            return domainLicenses.Distinct(new DomainLicenseEqualityComparer()).ToList();
        }

        private void SaveDomainLicenses(IEnumerable<DomainLicense> domainLicenses)
        {
            foreach (DomainLicense domainLicense in domainLicenses)
            {
                if (domainLicense.IsNew)
                {
                    context.DomainLicenses.Add(domainLicense);
                }
            }

            context.SaveChanges();
        }

        private IEnumerable<DomainValidationResult> ToDomainVelidationResults(IEnumerable<DomainLicense> domainLicenses)
        {
            return domainLicenses.Select(x => new DomainValidationResult
            {
                DomainName = x.DomainName,
                Expires = x.DomainLicenseExpires,
                Issued = x.DomainLicenseIssued,
                OwnerName = x.License.OwnerName,
                Features = GetFeatureCodes(x.License).ToList()
            });
        }

        private static IEnumerable<Guid> GetFeatureCodes(License license)
        {
            return license.Sku.SkuFeatures.Select(s => s.Feature.FeatureCode);
        }

        /// <summary>
        /// Validate licenses
        /// </summary>
        /// <param name="appKey">AppKey from CustomerAppKeys table</param>
        /// <param name="domainValidations"></param>
        /// <returns></returns>
        public static IEnumerable<DomainValidationResult> ValidateLicense(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            using (var licenseValidator = new LicenseValidator())
            {
                return licenseValidator.Validate(appKey, domainValidations);
            }
        }
    }
}
