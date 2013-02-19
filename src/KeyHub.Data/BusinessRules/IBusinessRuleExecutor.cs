using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    public interface IBusinessRuleExecutor
    {
        /// <summary>
        /// Executes the business rules a given entity and db context
        /// </summary>
        /// <param name="entity">The entity to apply business rules to</param>
        /// <param name="entityEntry">
        /// The original entity entry from the validation call. 
        /// Can be null when the rule is called from outside the context
        /// </param>
        /// <returns>A list of ValidationResult</returns>
        IEnumerable<BusinessRuleValidationResult> ExecuteBusinessResult(IModelItem entity, DbEntityEntry entityEntry);
    }
}