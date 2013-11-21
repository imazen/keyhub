using KeyHub.Common;
using KeyHub.Core.Logging;
using KeyHub.Core.UnitOfWork;
using KeyHub.Data;
using KeyHub.Data.ApplicationIssues;
using KeyHub.Data.BusinessRules;
using KeyHub.Data.Extensions;
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
    public class LicenseValidator : ILicenseValidator
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly ILoggingService loggingService;
        private readonly IApplicationIssueUnitOfWork applicationIssueUnitOfWork;

        private CustomerApp customerApp;

        public LicenseValidator(IDataContextFactory dataContextFactory, ILoggingService loggingService, IApplicationIssueUnitOfWork applicationIssueUnitOfWork)
        {
            this.dataContextFactory = dataContextFactory;
            this.loggingService = loggingService;
            this.applicationIssueUnitOfWork = applicationIssueUnitOfWork;
        }

        /// <summary>
        /// Vaidate DomainLicenses
        /// </summary>
        /// <param name="appKey">Customer App key</param>
        /// <param name="domainValidations">List of domains to validate</param>
        /// <returns>List of DomainValidationResult</returns>
        public IEnumerable<DomainValidationResult> Validate(Guid appKey, IEnumerable<DomainValidation> domainValidations)
        {
            using (var context = dataContextFactory.Create())
            {
                DeleteExpiredDomainLicenses();

                customerApp = context.CustomerAppKeys.Where(x => x.AppKey == appKey)
                                     .Select(x => x.CustomerApp)
                                     .Include(x => x.LicenseCustomerApps)
                                     .FirstOrDefault();

                if (customerApp == null)
                {
                    // need to notify, no customerApp
                    throw new Exception(string.Format("CustomerApp with appKey={0} has not found", appKey));
                }

                IEnumerable<Guid> matchedLicenseIds = GetValidLicences(customerApp).ToList();

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
                        var featureLicense = (from x in context.Licenses where matchedLicenseIds.Contains(x.ObjectId) select x)
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
                                AutomaticallyCreated = true,
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
                return ConvertToDomainValidationResults(domainLicenses);
            }
        }


        /// <summary>
        /// Resolves valid licenses and adds issues for expiring and expired ones
        /// </summary>
        /// <param name="customerAppliccation">Customer app to get licenses for</param>
        /// <returns>List of valid license guids</returns>
        private IEnumerable<Guid> GetValidLicences(CustomerApp customerAppliccation)
        {
            var licenses = (from x in customerAppliccation.LicenseCustomerApps select x.License);

            foreach (var license in licenses)
            {
                if (!license.LicenseExpires.HasValue)
                    continue;
                
                if (license.LicenseExpires.Value < DateTime.Now)
                {
                    applicationIssueUnitOfWork.CustomerAppId = customerApp.CustomerAppId;
                    applicationIssueUnitOfWork.DateTime = DateTime.Now;
                    applicationIssueUnitOfWork.Severity = ApplicationIssueSeverity.High;
                    applicationIssueUnitOfWork.Message = "License expired";
                    applicationIssueUnitOfWork.Details = String.Format("Your license has expired at: {0}",
                                                                       license.LicenseExpires);
                    applicationIssueUnitOfWork.Commit();
                }
                else if (license.LicenseExpires.Value.AddDays(Constants.LicenseExpireWarningDays*-1) < DateTime.Now)
                {
                    applicationIssueUnitOfWork.CustomerAppId = customerApp.CustomerAppId;
                    applicationIssueUnitOfWork.DateTime = DateTime.Now;
                    applicationIssueUnitOfWork.Severity = ApplicationIssueSeverity.Medium;
                    applicationIssueUnitOfWork.Message = "License is about to expire";
                    applicationIssueUnitOfWork.Details = String.Format("Your license is about to expire at: {0}",
                                                                       license.LicenseExpires);
                    applicationIssueUnitOfWork.Commit();
                }
                else
                {
                    yield return license.ObjectId;
                }
            }
        }

        public void OnValidationFailed(BusinessRuleValidationException businessRuleValidationException)
        {
            foreach (var error in businessRuleValidationException.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
            {
                loggingService.Log(error.ErrorMessage);

                applicationIssueUnitOfWork.CustomerAppId = customerApp.CustomerAppId;
                applicationIssueUnitOfWork.DateTime = DateTime.Now;
                applicationIssueUnitOfWork.Severity = ApplicationIssueSeverity.High;
                applicationIssueUnitOfWork.Message = error.BusinessRuleName;
                applicationIssueUnitOfWork.Details = error.ErrorMessage;
                applicationIssueUnitOfWork.Commit();
 			}
		}
        

        private IEnumerable<DomainValidationResult> ConvertToDomainValidationResults(IEnumerable<DomainLicense> domainLicenses)
        {
            return domainLicenses.Select(x => new DomainValidationResult
            {
                DomainLicense = new KeyHub.Client.DomainLicense(x.DomainName, x.License.OwnerName, x.DomainLicenseIssued,  x.DomainLicenseExpires, GetFeatureCodes(x.License).ToList()),
                KeyBytes = x.KeyBytes,
            }).ToList();
        }

        private void DeleteExpiredDomainLicenses()
        {
            using (var context = dataContextFactory.Create())
            {
                var expiredDomainLicenses = context.DomainLicenses
                                                   .AutomaticallyCreated()
                                                   .Expired()
                                                   .ToList();

                foreach (var expiredDomainLicense in expiredDomainLicenses)
                {
                    // notify
                    loggingService.Log(string.Format("Domain expired: id: {0}, name: {1}, licenseId: {2}", 
                                                     expiredDomainLicense.DomainLicenseId,
                                                     expiredDomainLicense.DomainName,
                                                     expiredDomainLicense.LicenseId), 
                                                     LogTypes.Info);
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
