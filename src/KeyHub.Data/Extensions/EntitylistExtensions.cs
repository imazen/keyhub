using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KeyHub.Data
{
    public static class EntitylistExtensions
    {
        /// <summary>
        /// Convert a enumerable of entities to a summary string. 
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity queried</typeparam>
        /// <param name="query">Query statement</param>
        /// <param name="displayNavigator">Expression to the display member</param>
        /// <param name="maxItems">Maximum items to show</param>
        /// <param name="separator">Separator between individual items</param>
        /// <returns>Selectionlist with query results in name, value pairs</returns>
        public static string ToSummary<TEntity>(this IEnumerable<TEntity> query,
            Func<TEntity, string> displayNavigator, int maxItems, string separator)
        {
            string summary = "";

            int totalCountDatabase = query.Count();
            var filteredItems = query.Take(maxItems);

            if (filteredItems.Count() > 0)
            {
                summary = string.Join(separator, filteredItems.Select(displayNavigator));
                if (totalCountDatabase > maxItems)
                    summary += string.Format(" and {0} more...", totalCountDatabase - maxItems);
            }
            else
                summary = "None";

            return summary;
        }

        /// <summary>
        /// Removes all matched entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="predicate"></param>
        public static void Remove<TEntity>(this DbSet<TEntity> entitySet, Func<TEntity, bool> predicate) where TEntity : class
        {
            foreach (var entity in entitySet.Where(predicate))
            {
                entitySet.Remove(entity);
            }
        }

        /// <summary>
        /// Removes all passed entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entities"></param>
        public static void Remove<TEntity>(this DbSet<TEntity> entitySet, IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var entity in entities)
            {
                entitySet.Remove(entity);
            }
        }
    }
}
