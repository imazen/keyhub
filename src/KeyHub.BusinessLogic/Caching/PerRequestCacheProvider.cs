using System;
using System.Collections.Generic;
using System.Linq;
using KeyHub.Core.Caching;
using KeyHub.Core.Logging;

namespace KeyHub.BusinessLogic.Caching
{
    /// <summary>
    /// Provides per request (HttpContext.Items) caching
    /// </summary>
    public class PerRequestCacheProvider : IPerRequestCacheProvider
    {
        private readonly ILoggingService loggingService;

        public PerRequestCacheProvider(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        private void StoreObjectIntoCache(object cacheObject, string cacheKey, System.Web.HttpContext context)
        {
            if (cacheObject != null)
                loggingService.Debug("Storing object {0} into context cache", cacheKey);

            // Only add to the cache when the object is not null
            if (cacheObject != null)
                context.Items.Add(cacheKey, cacheObject);
        }

        public T GetObjectFromCache<T>(string cacheKey, System.Web.HttpContext context)
        {
            loggingService.Debug("Getting object {0} from context cache", cacheKey);

            object cacheItem = context.Items[cacheKey];

            if (cacheItem == null)
                return default(T);
            else
                return (T)cacheItem;
        }

        public T GetOrStoreObjectFromCache<T>(string cacheKey, System.Web.HttpContext context, Core.Caching.CacheModes cacheMode, int minutesToLive, System.Web.Caching.CacheItemPriority priority, Func<T> function)
        {
            object cacheItem = context.Items[cacheKey];

            if (cacheItem == null)
            {
                // Store the object
                T cacheObject = function.Invoke();
                StoreObjectIntoCache(cacheObject,
                                     cacheKey,
                                     context);

                return cacheObject;
            }
            else
            {
                loggingService.Debug("Getting object {0} from context cache", cacheKey);

                return (T)cacheItem;
            }
        }

        public void RemoveFromCache(string cacheKey, System.Web.HttpContext context)
        {
            context.Items.Remove(cacheKey);
        }

        public void RemoveFromCachePrefix(string cacheKeyPrefix, System.Web.HttpContext context)
        {
            var cacheList = new List<string>();
            var cacheEnumerator = context.Items.GetEnumerator();

            // Fetch all keys from item list
            while (cacheEnumerator.MoveNext())
            {
                cacheList.Add(cacheEnumerator.Key.ToString());
            }

            // Remove from cache if it starts with our prefix
            foreach (var key in cacheList.Where(x => x.StartsWith(cacheKeyPrefix)))
            {
                context.Items.Remove(key);
            }
        }

        public void Dispose()
        {
            // Nothing
        }
    }
}