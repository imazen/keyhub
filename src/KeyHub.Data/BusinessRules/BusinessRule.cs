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
    /// Base class for the IBusinessRule interfaces. Use this class to create your own business rules
    /// </summary>
    public abstract class BusinessRule<TEntity> : IBusinessRule<TEntity>
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
        protected abstract IEnumerable<BusinessRuleValidationResult> ExecuteValidation(TEntity entity, DbEntityEntry entityEntry);

        #region "IBusinessRule members"

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
        public IEnumerable<BusinessRuleValidationResult> Validate(TEntity entity, DbEntityEntry entityEntry)
        {
            return ExecuteValidation(entity, entityEntry);
        }

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
        public IEnumerable<BusinessRuleValidationResult> Validate(IModelItem entity, DbEntityEntry entityEntry)
        {
            return ExecuteValidation((TEntity)entity, entityEntry);
        }

        /// <summary>
        /// Gets the business rule name
        /// </summary>
        public abstract string BusinessRuleName { get; }
        
        #endregion
    }
}
