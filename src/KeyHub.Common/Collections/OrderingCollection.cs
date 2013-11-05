using System;
using System.Linq;

namespace KeyHub.Common.Collections
{
    /// <summary>
    /// Represents an ordered collection based on the AdaptingCollection
    /// </summary>
    public class OrderingCollection<T, M> : AdaptingCollection<T, M>
    {
        public OrderingCollection(Func<Lazy<T, M>, object> keySelector, bool descending = false)
            : base(e => descending ? e.OrderByDescending(keySelector) : e.OrderBy(keySelector))
        {
        }
    }
}