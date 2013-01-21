using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;

namespace KeyHub.BusinessLogic.BusinessRules.LicenseValidation
{
    /// <summary>
    /// Checks maximum domains count 
    /// </summary>
    public class MaxDomainsCountRule : BusinessRule<DomainLicense, DataContext>
    {
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(DomainLicense entity, DataContext context, DbEntityEntry entityEntry)
        {
            if (entity.License.Sku.MaxDomains.HasValue 
                && !context.DomainLicenses.Any(x => x.DomainName == entity.DomainName && x.LicenseId == entity.LicenseId))
            {
                int usedDomainsCount = context.DomainLicenses.Count(x => x.LicenseId == entity.LicenseId);
                int maxDomains = entity.License.Sku.MaxDomains.Value;

                if (usedDomainsCount < maxDomains)
                {
                    yield return BusinessRuleValidationResult.Success;
                }
                else
                {
                    yield return new BusinessRuleValidationResult(string.Format("MaxDomains violation: domain: {0}, licenseId: {1}", entity.DomainName, entity.LicenseId));
                }
            }
            else
            {
                yield return BusinessRuleValidationResult.Success;
            }
        }

        public override string BusinessRuleName
        {
            get { return "Maximum domains count violation"; }
        }
    }
}
