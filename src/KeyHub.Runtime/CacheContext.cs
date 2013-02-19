using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Caching;

namespace KeyHub.Runtime
{
    ///// <summary>
    ///// Proivdes runtime access to the cache providers
    ///// </summary>
    //public sealed class CacheContext
    //{
    //    #region "Singleton"

    //    /// <summary>
    //    /// Gets the current instance of the CacheContext class
    //    /// </summary>
    //    public static CacheContext Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                lock (instanceLock)
    //                {
    //                    if (instance == null)
    //                        instance = new CacheContext();
    //                }
    //            }

    //            return instance;
    //        }
    //    }

    //    private static volatile CacheContext instance;
    //    private static object instanceLock = new object();

    //    private CacheContext()
    //    {
    //        DependencyContext.Instance.Compose(this);
    //    }

    //    #endregion "Singleton"

    //    #region "Cache providers"

    //    /// <summary>
    //    /// Provides access to the current Per Request cache provider
    //    /// </summary>
    //    [Import(typeof(IPerRequestCacheProvider))]
    //    public IPerRequestCacheProvider PerRequestCache { get; private set; }

    //    /// <summary>
    //    /// Provides access to the current Runtime cache provider
    //    /// </summary>
    //    [Import(typeof(IRuntimeCacheProvider))]
    //    public IRuntimeCacheProvider RuntimeCache { get; private set; }

    //    #endregion "Cache providers"
    //}
}