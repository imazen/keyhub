using System.Linq.Expressions;
using KeyHub.BusinessLogic.LicenseValidation.Validators;
using KeyHub.Core.Logging;
using KeyHub.Data;
using KeyHub.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KeyHub.Runtime;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Validates licenses
    /// </summary>
    public class LicenseValidator : IDisposable
    {
        private readonly DataContext context;

        private readonly List<IValidator> validators;

        private LicenseValidator()
        {
            context = new DataContext();
            validators = new List<IValidator>
            {
                new MaxDomainsCountValidator()
            };
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

            domainLicenses = validators.Aggregate(domainLicenses, (current, validator) => validator.Validate(current));

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
                throw new Exception(string.Format("CustomerApp with appKey={0} has not found", appKey));
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

            return domainLicenses
                .Distinct(new DomainLicenseEqualityComparer())
                .OrderByDescending(x => x.DomainLicenseId) // in order to validate already stored first 
                .ToList();
        }

        private void SaveDomainLicenses(IEnumerable<DomainLicense> domainLicenses)
        {
            foreach (DomainLicense domainLicense in domainLicenses.Where(domainLicense => domainLicense.IsNew))
            {
                context.DomainLicenses.Add(domainLicense);
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
                KeyBytes = x.KeyBytes,
                Features = GetFeatureCodes(x.License).ToList()
            }).ToList();
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
