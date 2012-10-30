using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules.Rules
{
    class UniqueFeatureCodeRule : BusinessRule<Feature, DataContext>
    {
        /// <summary>
        /// Validates the entity to ensure this business rule is applied before 
        /// being saved to the database
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="context">The data context to use</param>
        /// <returns>A collection of errors, or an empty collection if the business rule succeeded</returns>
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(Feature entity, DataContext context, DbEntityEntry entityEntry)
        {

            var duplicateFeatureCode = 
                (from x in context.Features 
                 where x.FeatureCode == entity.FeatureCode && x.VendorId==entity.VendorId && x.FeatureId != entity.FeatureId
                 select x)
                .Include(x => x.SkuFeatures.Select(f => f.Sku)).FirstOrDefault();

            if (duplicateFeatureCode != null)
            {
                if (duplicateFeatureCode.SkuFeatures.Any())
                {
                    yield return
                        new BusinessRuleValidationResult(
                            string.Format("Feature Code already used for feature '{0}' on SKU '{1}'.",
                                          duplicateFeatureCode.FeatureName,
                                          duplicateFeatureCode.SkuFeatures.First().Sku.SkuCode), this, "FeatureCode");
                }
                else
                {
                    yield return
                            new BusinessRuleValidationResult(
                                string.Format("Feature Code already used for feature '{0}'.",
                                              duplicateFeatureCode.FeatureName), this, "FeatureCode");
                }
            }

            yield return BusinessRuleValidationResult.Success;
        }

        /// <summary>
        /// Gets the business rule name
        /// </summary>
        public override string BusinessRuleName
        {
            get { return "Unique Feature Code"; }
        }
    }
}
