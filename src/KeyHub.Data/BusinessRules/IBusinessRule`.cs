using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    /// Interface to interact with the validation system of the datacontext.
    /// These business rules will run before the data is saved to the database
    /// </summary>
    /// <typeparam name="TEntity">The entity type for this business rule</typeparam>
    public interface IBusinessRule<in TEntity> : IBusinessRule
         where TEntity : IModelItem
    {
        /// <summary>
        /// Validates the entity to ensure this business rule is applied before 
        /// being saved to the database
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="entityEntry">
        /// The original entity entry from the validation call. 
        /// Can be null when the rule is called from outside the context
        /// </param>
        /// <returns>A collection of errors, or an empty collection if the business rule succeeded</returns>
        IEnumerable<BusinessRuleValidationResult> Validate(TEntity entity, DbEntityEntry entityEntry);
    }   
}
