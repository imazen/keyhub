using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    /// <summary>
    /// Provides utility methods for executing business rules
    /// </summary>
    internal static class BusinessRuleExecutor
    {
        /// <summary>
        /// Executes the business rules a given entity and db context
        /// </summary>
        /// <typeparam name="TContext">The type of the context</typeparam>
        /// <param name="entity">The entity to apply business rules to</param>
        /// <param name="context">The context to use when applying the business rules</param>
        /// <param name="entityEntry">
        /// The original entity entry from the validation call. 
        /// Can be null when the rule is called from outside the context
        /// </param>
        /// <returns>A list of ValidationResult</returns>
        internal static IEnumerable<BusinessRuleValidationResult> ExecuteBusinessResult<TContext>(IModelItem entity, TContext context, DbEntityEntry entityEntry)
            where TContext : DbContext
        {
            var businessRules = BusinessRulesContext.Instance.GetBusinessRules<TContext>(entity);
            foreach (var businessRule in businessRules)
            {
                foreach (var validationResult in businessRule.Validate(entity, context, entityEntry))
                {
                    yield return validationResult;
                }
            }
        }
    }
}
