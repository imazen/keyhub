using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub
{
    public static class RightsObjectExtensions
    {
        public static IQueryable<TEntity> FilterByUser<TEntity>(this IQueryable<TEntity> query, User user) where TEntity : RightObject
        {
            // TODO: Filter on rights table
            return query;
        }
    }
}
