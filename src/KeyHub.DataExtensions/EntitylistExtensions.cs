using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub
{
    public static class EntitylistExtensions
    {

        /// <summary>
        /// Convert a query to a summary. 
        /// </summary>
        /// <typeparam name="TEntity">Type of the object queried (typed or untyped)</typeparam>
        /// <param name="query">Query statement</param>
        /// <param name="displayNavigator">Expression to the display member</param>
        /// <param name="maxItems">Maximum items to show</param>
        /// <param name="separator">Separator between individual items</param>
        /// <returns>Selectionlist with query results in name, value pairs</returns>
        public static string ToSummary<TEntity>(this IEnumerable<TEntity> query,
            Func<TEntity, string> displayNavigator, int maxItems, string separator)
        {
            string summary = "";

            var filteredItems = query.Take(maxItems);

            if (filteredItems.Count() > 0)
            {
                summary = string.Join(separator, filteredItems.Select(displayNavigator));
                if (query.Count() > maxItems)
                    summary += string.Format(" and {0} more...", query.Count() - maxItems);
            }
            else
                summary = "None";

            return summary;
        }
    }
}
