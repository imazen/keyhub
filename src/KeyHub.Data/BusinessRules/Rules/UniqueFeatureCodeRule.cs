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
    public class UniqueFeatureCodeRule : BusinessRule<Feature>
    {
        private readonly IDataContextFactory dataContextFactory;

        public UniqueFeatureCodeRule(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(Feature entity, DbEntityEntry entityEntry)
        {
            using (var fullContext = dataContextFactory.Create())
            {
                var duplicateFeatureCode =
                    (from x in fullContext.Features
                     where
                         x.FeatureCode == entity.FeatureCode && x.VendorId == entity.VendorId &&
                         x.FeatureId != entity.FeatureId
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
                                              duplicateFeatureCode.SkuFeatures.First().Sku.SkuCode), this, "FeatureCode")
                            ;
                    }
                    else
                    {
                        yield return
                            new BusinessRuleValidationResult(
                                string.Format("Feature Code already used for feature '{0}'.",
                                              duplicateFeatureCode.FeatureName), this, "FeatureCode");
                    }
                }
            }

            yield return BusinessRuleValidationResult.Success;
        }

        public override string BusinessRuleName
        {
            get { return "Unique Feature Code"; }
        }
    }
}
