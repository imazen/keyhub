using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using Castle.Windsor;
using Castle.Windsor.Installer;
using KeyHub.Core.Kernel;

namespace KeyHub.Runtime
{
    // TODO: Store this

    // KernelContext.Instance.RunKernelEvents(KernelEventsTypes.Startup);





    ///// <summary>
    ///// Provides runtime access to all Application scoped components
    ///// </summary>
    //public sealed class ApplicationContext : IDisposable
    //{
    //    #region "Singleton"

    //    /// <summary>
    //    /// Gets the current instance of the ApplicationContext class
    //    /// </summary>
    //    public static ApplicationContext Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                lock (instanceLock)
    //                {
    //                    if (instance == null)
    //                        instance = new ApplicationContext();
    //                }
    //            }

    //            return instance;
    //        }
    //    }

    //    private static volatile ApplicationContext instance;
    //    private static object instanceLock = new object();

    //    private ApplicationContext()
    //    {
    //        // Setup DI container
    //        InjectMef();
    //    }

    //    #endregion "Singleton"

    //    #region "MEF"

    //    /// <summary>
    //    /// Injects the ApplicationContext class with all Imports using MEF
    //    /// </summary>
    //    private void InjectMef()
    //    {
    //        // Compose this class using MEF
    //        DependencyContext.Instance.Compose(this);
    //    }

    //    #endregion "MEF"

    //    #region "Boot"

    //    /// <summary>
    //    /// Boots up the KeyHub application. Will throw exceptions to stop executing application
    //    /// </summary>
    //    public void Boot()
    //    {
    //        Runtime.LogContext.Instance.Info("Application started at {0}",
    //                                                DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

    //        // Run boot classes
            
    //    }

    //    #endregion "Boot"

    //    #region "Dispose"

    //    public void Dispose()
    //    {
    //    }

    //    #endregion "Dispose"
    //}
}