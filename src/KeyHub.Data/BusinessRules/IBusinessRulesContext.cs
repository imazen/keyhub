using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    /// <summary>
    /// Provides business rules storing and queriying
    /// </summary>
    public interface IBusinessRulesContext : IDisposable
    {
        /// <summary>
        /// Gets the business rules for the given entity
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to get the rules for</typeparam>
        /// <typeparam name="TContext">The type of the datacontext</typeparam>
        /// <returns>A set of rules associated with the given entity</returns>
        IEnumerable<IBusinessRule> GetBusinessRules<TEntity, TContext>()
            where TEntity : IModelItem
            where TContext : DbContext;

        /// <summary>
        /// Gets the business rules for the given entity
        /// </summary>
        /// <typeparam name="TContext">The type of the datacontext</typeparam>
        /// <returns>A set of rules associated with the given entity</returns>
        IEnumerable<IBusinessRule> GetBusinessRules<TContext>(IModelItem entity)
            where TContext : DbContext;
    }
}
