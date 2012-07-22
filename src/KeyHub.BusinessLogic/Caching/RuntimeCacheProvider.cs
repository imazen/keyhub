using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Caching;

namespace KeyHub.BusinessLogic.Caching
{
    /// <summary>
    /// Provides full application (HttpContext.Cache) caching
    /// </summary>
    public class RuntimeCacheProvider : IRuntimeCacheProvider
    {
        private void StoreObjectIntoCache(object cacheObject, string cacheKey, System.Web.HttpContext context, int MinutesToLive, CacheModes cacheMode, System.Web.Caching.CacheItemPriority priority)
        {
            // Only add to the cache when the object is not null
            if (cacheObject != null)
            {
                Runtime.LogContext.Instance.Debug("Storing {0} object {1} into cache", cacheMode.ToString(), cacheKey);

                // Different insert for different cache mode
                switch (cacheMode)
                {
                    case CacheModes.Absolute:
                        context.Cache.Insert(cacheKey,
                                  cacheObject,
                                  null,
                                  DateTime.Now.AddMinutes(MinutesToLive),
                                  System.Web.Caching.Cache.NoSlidingExpiration,
                                  priority,
                                  null);
                        break;

                    case CacheModes.Sliding:
                        context.Cache.Insert(cacheKey,
                                  cacheObject,
                                  null,
                                  System.Web.Caching.Cache.NoAbsoluteExpiration,
                                  new TimeSpan(0, MinutesToLive, 0),
                                  priority,
                                  null);
                        break;
                }
            }
        }

        public T GetObjectFromCache<T>(string cacheKey, System.Web.HttpContext context)
        {
            Runtime.LogContext.Instance.Debug("Getting object {0} from runtime cache", cacheKey);

            object cacheItem = context.Cache[cacheKey];

            if (cacheItem == null)
                return default(T);
            else
                return (T)cacheItem;
        }

        public T GetOrStoreObjectFromCache<T>(string cacheKey, System.Web.HttpContext context, Core.Caching.CacheModes cacheMode, int minutesToLive, System.Web.Caching.CacheItemPriority priority, Func<T> function)
        {
            object cacheItem = context.Cache[cacheKey];

            if (cacheItem == null)
            {
                // Store the object
                T cacheObject = function.Invoke();
                StoreObjectIntoCache(cacheObject,
                                     cacheKey,
                                     context,
                                     minutesToLive,
                                     cacheMode,
                                     priority);

                return cacheObject;
            }
            else
            {
                Runtime.LogContext.Instance.Debug("Getting object {0} from context cache", cacheKey);

                return (T)cacheItem;
            }
        }

        public void RemoveFromCache(string cacheKey, System.Web.HttpContext context)
        {
            context.Cache.Remove(cacheKey);
        }

        public void RemoveFromCachePrefix(string cacheKeyPrefix, System.Web.HttpContext context)
        {
            List<string> cacheList = new List<string>();
            var cacheEnumerator = context.Cache.GetEnumerator();

            // Fetch all keys from item list
            while (cacheEnumerator.MoveNext())
            {
                cacheList.Add(cacheEnumerator.Key.ToString());
            }

            // Remove from cache if it starts with our prefix
            foreach (string key in cacheList.Where(x => x.StartsWith(cacheKeyPrefix)))
            {
                context.Cache.Remove(key);
            }
        }

        public void Dispose()
        {
            // Nothing
        }
    }
}