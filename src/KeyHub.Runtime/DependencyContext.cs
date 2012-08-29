using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Conventions;
using MefContrib.Hosting.Conventions.Configuration;
using KeyHub.Core.Dependency;

namespace KeyHub.Runtime
{
    /// <summary>
    /// Holds the container and catalog for MEF and provides basic functionality for Dependency Injection
    /// </summary>
    public sealed class DependencyContext : IDisposable
    {
        #region "Singleton"

        /// <summary>
        /// Singleton instance of ClassInjector
        /// </summary>
        public static DependencyContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new DependencyContext();
                    }
                }

                return instance;
            }
        }

        private static volatile DependencyContext instance;
        private static object instanceLock = new object();

        /// <summary>
        /// Composes the container and sets up injection
        /// </summary>
        /// <param name="composable"></param>
        public DependencyContext()
        {
            SetupMef();
        }

        /// <summary>
        /// Disposes the container and catalog
        /// </summary>
        public void Dispose()
        {
            if (configCatalog != null)
                configCatalog.Dispose();

            if (container != null)
                container.Dispose();

            if (catalog != null)
                catalog.Dispose();
        }

        #endregion "Singleton"

        #region "MEF - External"

        /// <summary>
        /// Returns a exported value from the internal MEF container
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <returns>An instance of the Type, or null if not found</returns>
        public T GetExportedValue<T>()
        {
            return container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// Returns a exported value from the internal MEF container
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <returns>An instance of the Type, or null if not found</returns>
        public IEnumerable<T> GetExportedValues<T>()
        {
            return container.GetExportedValues<T>();
        }

        /// <summary>
        /// Composes the classes and injects the correct imports
        /// </summary>
        /// <param name="composables">An array of classes to compose</param>
        public void Compose(params object[] composables)
        {
            if (container != null)
                container.ComposeParts(composables);
        }

        #endregion "MEF - External"

        #region "MEF - Internal"

        private AggregateCatalog catalog;
        private CompositionContainer container;
        private ConventionCatalog configCatalog;

        private void SetupMef()
        {
            // Force load all assemblies
            DeployedAssemblyLoader.PreLoadDeployedAssemblies();

            // Create a new aggregate catalog to support multiple catalogs
            catalog = new AggregateCatalog();

            // Get all parts from the configuration file
            configCatalog = new ConventionCatalog(new ConfigurationPartRegistry("Mef.Configuration"));

            // Get all parts from the current appdomain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.GlobalAssemblyCache)
                    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
            
            // Add them to the catalog
            catalog.Catalogs.Add(configCatalog);

            // Create the composition container
            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
        }

        #endregion "MEF - Internal"
    }
}