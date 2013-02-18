using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Data.Extensions;
using KeyHub.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace KeyHub.BusinessLogic.BusinessRules.LicenseValidation
{
    /// <summary>
    /// Checks maximum domains count 
    /// </summary>
    public class MaxDomainsCountRule : BusinessRule<DomainLicense, DataContext>
    {
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(DomainLicense entity, DataContext context, DbEntityEntry entityEntry)
        {
            SKU sku = context.Licenses
                .Include(x => x.Sku)
                .Where(x => x.ObjectId == entity.LicenseId)
                .Select(x => x.Sku)
                .FirstOrDefault();

            if (sku == null)
                yield return new BusinessRuleValidationResult("Sku could not be resolved for license.", this, null);

            if (sku.MaxDomains.HasValue && !context.DomainLicenses.Any(x => x.DomainLicenseId == entity.DomainLicenseId))
            {
                int usedDomainsCount = context.DomainLicenses.Count(x => x.LicenseId == entity.LicenseId);
                int maxDomains = sku.MaxDomains.Value;

                if (usedDomainsCount < maxDomains)
                {
                    yield return BusinessRuleValidationResult.Success;
                }
                else
                {
                    yield return new BusinessRuleValidationResult("Domain cannot be added. This license reached maximum domains count violation.", this, null);
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
