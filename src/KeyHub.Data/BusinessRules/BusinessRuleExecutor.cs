using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Common.Utils;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    public class BusinessRuleExecutor : IBusinessRuleExecutor
    {
        private readonly IEnumerable<IBusinessRule> businessRules;

        public BusinessRuleExecutor(IEnumerable<IBusinessRule> businessRules)
        {
            this.businessRules = businessRules;
        }

        public IEnumerable<BusinessRuleValidationResult> ExecuteBusinessResult(IModelItem entity, DbEntityEntry entityEntry)
        {
            return businessRules.Where(x => Reflection.GetGenericParameters(x).Contains(entity.GetType()))
                                .SelectMany(businessRule => businessRule.Validate(entity, entityEntry));
        }
    }
}
