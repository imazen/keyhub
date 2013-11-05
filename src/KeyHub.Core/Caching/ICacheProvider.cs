using System;
using System.Web;

namespace KeyHub.Core.Caching
{
    /// <summary>
    /// Provides basic caching functionality
    /// </summary>
    public interface ICacheProvider : IDisposable
    {
        /// <summary>
        /// Gets an object from the underlying cache store
        /// </summary>
        /// <typeparam name="T">The type of the return object</typeparam>
        /// <param name="cacheKey">The key of the cache item</param>
        /// <param name="context">The current HttpContext</param>
        T GetObjectFromCache<T>(string cacheKey, HttpContext context);

        /// <summary>
        /// Gets or inserts an object from/into the underlying cache store
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="cacheKey">The key of the cache item</param>
        /// <param name="context">The current HttpContext</param>
        /// <param name="cacheMode">What type of caching to use</param>
        /// <param name="minutesToLive">The ammount of minutes the cache has to stay alive</param>
        /// <param name="priority">The priority of the cache object</param>
        /// <param name="function">Delegate to retrieve the object</param>
        T GetOrStoreObjectFromCache<T>(string cacheKey, HttpContext context, CacheModes cacheMode, int minutesToLive, System.Web.Caching.CacheItemPriority priority, Func<T> function);

        /// <summary>
        /// Removes an object from the underlying cache store by its key
        /// </summary>
        /// <param name="cacheKey">The key of the cache item</param>
        /// <param name="context">The current HttpContext</param>
        void RemoveFromCache(string cacheKey, HttpContext context);

        /// <summary>
        /// Removes an all object from the underlying cache that start with
        /// </summary>
        /// <param name="cacheKeyPrefix">The key of the cache item</param>
        /// <param name="context">The current HttpContext</param>
        void RemoveFromCachePrefix(string cacheKeyPrefix, HttpContext context);
    }
}