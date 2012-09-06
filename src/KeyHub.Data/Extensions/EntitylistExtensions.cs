using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
    }
}
