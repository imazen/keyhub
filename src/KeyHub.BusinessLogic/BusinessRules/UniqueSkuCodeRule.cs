using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;

namespace KeyHub.BusinessLogic.BusinessRules
{
    public class UniqueSkuCodeRule : BusinessRule<SKU>
    {
        private readonly IDataContextFactory dataContextFactory;

        public UniqueSkuCodeRule(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Validates the entity to ensure this business rule is applied before 
        /// being saved to the database
        /// </summary>
        /// <returns>A collection of errors, or an empty collection if the business rule succeeded</returns>
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(SKU entity,  DbEntityEntry entityEntry)
        {
            using (var context = dataContextFactory.Create())
            {
                var duplicateSkuCode =
               (from x in context.SKUs
                where (x.SkuCode == entity.SkuCode || x.SkuAternativeCode == entity.SkuCode)
                   && x.VendorId == entity.VendorId && x.SkuId != entity.SkuId
                select x)
               .FirstOrDefault();

                var duplicateSkuAlternativeCode =
                    (from x in context.SKUs
                     where (x.SkuCode == entity.SkuAternativeCode || x.SkuAternativeCode == entity.SkuAternativeCode)
                        && x.VendorId == entity.VendorId && x.SkuId != entity.SkuId
                     select x)
                    .FirstOrDefault();

                if (duplicateSkuCode != null)
                {
                    yield return
                        new BusinessRuleValidationResult(
                            string.Format("SKU Code already used for SKU '{0}'.",
                                            duplicateSkuCode.SkuCode), this, "SkuCode");
                }
                if (duplicateSkuAlternativeCode != null)
                {
                    yield return
                            new BusinessRuleValidationResult(
                                string.Format("SKU Alternative Code already used for SKU '{0}'.",
                                                duplicateSkuAlternativeCode.SkuCode), this, "SkuAternativeCode");
                }
            }
           

            yield return BusinessRuleValidationResult.Success;
        }

        /// <summary>
        /// Gets the business rule name
        /// </summary>
        public override string BusinessRuleName
        {
            get { return "Unique SKU Code"; }
        }
    }
}
