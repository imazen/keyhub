using KeyHub.Core.Logging;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Data.Extensions;
using KeyHub.Model;
using KeyHub.Runtime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Validates licenses
    /// </summary>
    public class LicenseValidator
    {
        private readonly IDataContextFactory dataContextFactory;

        /// <summary>
        /// Validate licenses
        /// </summary>
        /// <param name="dataContextFactory">Factory used by validator</param>
        /// <param name="appKey">AppKey from CustomerAppKeys table</param>
        /// <param name="domainValidations">Domains to validate</param>
        /// <returns>List of DomainValidationResult</returns>
        public static IEnumerable<DomainValidationResult> ValidateLicense(IDataContextFactory dataContextFactory, Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            var licenseValidator = new LicenseValidator(dataContextFactory);
            return licenseValidator.Validate(appKey, domainValidations);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextFactory">Facotry used by validator</param>
        private LicenseValidator(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Vaidate DomainLicenses
        /// </summary>
        /// <param name="appKey">Customer App key</param>
        /// <param name="domainValidations">List of domains to validate</param>
        /// <returns>List of DomainValidationResult</returns>
        private IEnumerable<DomainValidationResult> Validate(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            using (var context = dataContextFactory.Create())
            {
                DeleteExpiredDomainLicenses();

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

                var alreadyFailedDomainLicenses = new List<DomainLicense>();

                IEqualityComparer<DomainLicense> equalityComparer = new DomainLicenseEqualityComparer();

                foreach (DomainValidation domainValidation in domainValidations)
                {
                    string domainName = domainValidation.DomainName;
                    Guid featureCode = domainValidation.FeatureCode;

                    var domainLicense = context.DomainLicenses
                                               .Include(x => x.License.Sku.SkuFeatures.Select(s => s.Feature))
                                               .FirstOrDefault(x => x.DomainName == domainName
                                                                    && matchedLicenseIds.Contains(x.LicenseId)
                                                                    &&
                                                                    x.License.Sku.SkuFeatures.Select(
                                                                        s => s.Feature.FeatureCode)
                                                                     .Contains(featureCode));

                    if (domainLicense == null)
                    {
                        var featureLicense = (from x in context.Licenses select x)
                            .Include(x => x.Sku.SkuFeatures.Select(s => s.Feature))
                            .FirstOrDefault(
                                x => x.Sku.SkuFeatures.Select(s => s.Feature.FeatureCode).Any(y => y == featureCode));

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
                                DomainLicenseIssued = featureLicense.Sku.CalculateDomainIssueDate(),
                                DomainLicenseExpires = featureLicense.Sku.CalculateAutoDomainExpiration(),
                                KeyBytes = featureLicense.Sku.PrivateKey.KeyBytes,
                                License = featureLicense,
                                LicenseId = featureLicense.ObjectId
                            };

                        if (!context.DomainLicenses.Any(x => x.DomainLicenseId == domainLicense.DomainLicenseId) &&
                            !alreadyFailedDomainLicenses.Contains(domainLicense, equalityComparer))
                        {
                            context.DomainLicenses.Add(domainLicense);
                            if (!context.SaveChanges(OnValidationFailed))
                            {
                                alreadyFailedDomainLicenses.Add(domainLicense);
                            }
                        }
                    }

                    if (!domainLicenses.Contains(domainLicense, equalityComparer) &&
                        !alreadyFailedDomainLicenses.Contains(domainLicense, equalityComparer))
                    {
                        domainLicenses.Add(domainLicense);
                    }
                }
                return ToDomainVelidationResults(domainLicenses);
            }
        }

        public void OnValidationFailed(BusinessRuleValidationException businessRuleValidationException)
        {
            foreach (var error in businessRuleValidationException.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
            {
                LogContext.Instance.Log(error.ErrorMessage);
            }
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

        public void DeleteExpiredDomainLicenses()
        {
            using (var context = dataContextFactory.Create())
            {
                var expiredDomainLicenses = context.DomainLicenses
                                                   .AutomaticlyCreated()
                                                   .Expired()
                                                   .ToList();

                foreach (var expiredDomainLicense in expiredDomainLicenses)
                {
                    // notify
                    LogContext.Instance.Log(string.Format("Domain expired: id: {0}, name: {1}, licenseId: {2}"
                                                          , expiredDomainLicense.DomainLicenseId,
                                                          expiredDomainLicense.DomainName,
                                                          expiredDomainLicense.LicenseId), LogTypes.Info);
                }

                context.DomainLicenses.Remove(expiredDomainLicenses);
                context.SaveChanges();
            }
        }

        private static IEnumerable<Guid> GetFeatureCodes(License license)
        {
            return license.Sku.SkuFeatures.Select(s => s.Feature.FeatureCode);
        }
    }
}
