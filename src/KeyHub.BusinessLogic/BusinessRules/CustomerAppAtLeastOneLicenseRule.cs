using System.Data;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace KeyHub.BusinessLogic.BusinessRules
{
    public class CustomerAppAtLeastOneLicenseRule : BusinessRule<CustomerApp, DataContext>
    {
        /// <summary>
        /// Validates a CustomerApp to see if it has at least one license
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <param name="entityEntry"></param>
        /// <returns>A collection of errors, or an empty collection if the business rule succeeded</returns>
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(CustomerApp entity, DataContext context, DbEntityEntry entityEntry)
        {
            if (entity.LicenseCustomerApps.Count == 0 && entityEntry.State != EntityState.Deleted)
            {
                yield return new BusinessRuleValidationResult("Licensed application should have at least one license", this, "SelectedLicenseGUIDs");
            }

            yield return BusinessRuleValidationResult.Success;
        }

        /// <summary>
        /// Gets the business rule name
        /// </summary>
        public override string BusinessRuleName
        {
            get { return "CustomerApp should have at least one license"; }
        }
    }
}
