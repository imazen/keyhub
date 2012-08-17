using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Common.Utils;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    /// <summary>
    /// Provides business rules storing and queriying
    /// </summary>
    public class BusinessRulesContext : IBusinessRulesContext
    {
        #region "Singleton"

        /// <summary>
        /// Gets the current instance of the ApplicationContext class
        /// </summary>
        public static IBusinessRulesContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new BusinessRulesContext();
                    }
                }

                return instance;
            }
        }

        private static volatile IBusinessRulesContext instance;
        private static object instanceLock = new object();

        private BusinessRulesContext()
        {
            Runtime.DependencyContext.Instance.Compose(this);
            PrepareTypesRules();
        }

        #endregion "Singleton"
                
        public void Dispose()
        {
            rules = null;
            typedRules.Clear();
        }

        #region "Internal Rules"

        /// <summary>
        /// Represents all entity validation rules
        /// </summary>
        [ImportMany("BusinessRules", RequiredCreationPolicy=CreationPolicy.Shared)]
        private IEnumerable<IBusinessRule> rules;

        /// <summary>
        /// Represents a copy of the rules list, but indexed on the entity type
        /// </summary>
        private Dictionary<Type, List<IBusinessRule>> typedRules;

        /// <summary>
        /// Flattens the rules list by extracting the entity types and storing them in a dictionary
        /// </summary>
        private void PrepareTypesRules()
        {
            typedRules = new Dictionary<Type, List<IBusinessRule>>();

            foreach (var rule in rules)
            {
                ImportBusinessRule(rule, typedRules);
            }
        }

        /// <summary>
        /// Gets the actual entity type of the rules base class and inserts it into the dictionary
        /// </summary>
        /// <param name="rule">The rule to extract</param>
        /// <param name="list">The list to add the extracted rule to</param>
        private static void ImportBusinessRule(IBusinessRule rule, Dictionary<Type, List<IBusinessRule>> list)
        {
            var entityType = Reflection.GetGenericParameters(rule).FirstOrDefault();

            if (!list.ContainsKey(entityType))
                list.Add(entityType, new List<IBusinessRule>());

            list[entityType].Add(rule);
        }

        /// <summary>
        /// Extracts the entity from a class
        /// </summary>
        /// <param name="entity">The entity to extract from</param>
        /// <returns>The type of the entity</returns>
        /// <remarks>
        /// EntityFramework creates 'Proxy classes' (http://en.wikipedia.org/wiki/Proxy_pattern) for entities that 
        /// are loaded from the store. Becasue these proxies aren't actually the entity type we're looking for, 
        /// we extract them using reflection.
        /// </remarks>
        private static Type ExtractEntityType(IModelItem entity)
        {
            return Reflection.IsProxy(entity) ? Reflection.GetProxyUnderlyingType(entity) : entity.GetType();
        }

        #endregion

        #region "IBusinessRulesContext members"

        /// <summary>
        /// Gets the business rules for the given entity
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to get the rules for</typeparam>
        /// <typeparam name="TContext">The type of the datacontext</typeparam>
        /// <returns>A set of rules associated with the given entity</returns>
        public IEnumerable<IBusinessRule> GetBusinessRules<TEntity, TContext>()
            where TEntity : IModelItem
            where TContext : DbContext
        {
            if (typedRules.ContainsKey(typeof(TEntity)))
            {
                foreach (var rule in typedRules[typeof(TEntity)])
                {
                    yield return rule;
                }
            }
        }

        /// <summary>
        /// Gets the business rules for the given entity
        /// </summary>
        /// <typeparam name="TContext">The type of the datacontext</typeparam>
        /// <returns>A set of rules associated with the given entity</returns>
        public IEnumerable<IBusinessRule> GetBusinessRules<TContext>(IModelItem entity) where TContext : DbContext
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entityType = ExtractEntityType(entity);

            if (typedRules.ContainsKey(entityType))
            {
                foreach (var rule in typedRules[entityType])
                {
                    yield return rule;
                }
            }
        }

        #endregion
    }
}
