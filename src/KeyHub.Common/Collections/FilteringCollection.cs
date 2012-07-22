using System;
using System.Linq;

namespace KeyHub.Common.Collections
{
    /// <summary>
    /// Represents an filtered collection based on the AdaptingCollection
    /// </summary>
    public class FilteringCollection<T, M> : AdaptingCollection<T, M>
    {
        public FilteringCollection(Func<Lazy<T, M>, bool> filter)
            : base(e => e.Where(filter))
        {
        }
    }
}