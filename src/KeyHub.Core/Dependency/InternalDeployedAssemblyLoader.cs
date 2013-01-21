using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KeyHub.Core.Dependency
{
    /// <summary>
    /// Assembly loader class to help load all assemblies present with the application
    /// </summary>
    /// <remarks>
    /// Apparently, not all assemblies are loaded at once, but only after they are called by the system.
    /// When using DI on assemblies that haven't been loaded yet, this causes problems because the parts aren't 
    /// known when the container is initialized. The result is that some dependencies can't be resolved, even though they 
    /// are present.
    /// This also applies to non-web environments.
    /// Solution: 
    ///     Load all assemblies from the bin path (even for non-web environments) and explicitly load them into the Appdomain
    /// </remarks>
    public static class DeployedAssemblyLoader
    {
        /// <summary>
        /// Preloads all deployed assemblies into the current AppDomain
        /// </summary>
        public static void PreLoadDeployedAssemblies()
        {
            foreach (var path in GetBinFolders())
            {
                PreLoadAssembliesFromPath(path);
            }
        }

        private static IEnumerable<string> GetBinFolders()
        {
            // TODO: The AppDomain.CurrentDomain.BaseDirectory usage is not correct in 
            // some cases. Need to consider PrivateBinPath too
            List<string> assemblyFolders = new List<string>();
            
            // Check if we are on ASP.NET
            if (HttpContext.Current != null && HttpRuntime.AppDomainAppId != null)
            {
                assemblyFolders.Add(HttpRuntime.BinDirectory);
            }
            else
            {
                assemblyFolders.Add(AppDomain.CurrentDomain.BaseDirectory);
            }

            return assemblyFolders;
        }

        private static void PreLoadAssembliesFromPath(string path)
        {
            // Get all dll files from the given path and search child folders too
            FileInfo[] files = null;
            files = new DirectoryInfo(path).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fullName = file.FullName;
                AssemblyName assemblyName = null;

                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(fullName);
                }
                catch (BadImageFormatException)
                {
                    // Skip this assembly as this is clearly not a .NET DLL
                    continue;
                }                
                
                // Check if the assembly hasn't been loaded yet using it's definition
                if (!AppDomain.CurrentDomain.GetAssemblies()
                                                .Any(a =>
                                                    AssemblyName
                                                        .ReferenceMatchesDefinition(assemblyName, a.GetName())
                                                 )
                    )
                {
                    // Use the AssemblyName class to load the DLL for allround .NET support
                    Assembly.Load(assemblyName);
                }
            }
        }
    }
}
