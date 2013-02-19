using System.IO;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;

namespace KeyHub.Web.Composition
{
    /// <summary>
    /// Contains factory methods for the windsor container
    /// </summary>
    public static class CompositionContainerFactory
    {
        /// <summary>
        /// Adds default components to the container
        /// </summary>
        private static void AddDefaultComponents(IWindsorContainer container)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.AddFacility<TypedFactoryFacility>();
        }

        /// <summary>
        /// Creates the container using an optional config file
        /// </summary>
        /// <param name="configFileLocation">Optional config file</param>
        private static IWindsorContainer CreateContainer(string configFileLocation = "")
        {
            WindsorContainer windsorContainer;

            if (!string.IsNullOrEmpty(configFileLocation))
            {
                if (!File.Exists(configFileLocation))
                    throw new FileNotFoundException("Could not load Windsor configuration file.", configFileLocation);

                 windsorContainer = new WindsorContainer(new XmlInterpreter(configFileLocation));
            }
            else
            {
                windsorContainer = new WindsorContainer();
            }

            AddDefaultComponents(windsorContainer);

            return windsorContainer;
        }

        /// <summary>
        /// Creates a new Windsor container based on installers from the parameter
        /// </summary>
        /// <param name="installers">The installers to pass to the container</param>
        public static IWindsorContainer CreateWithInstallers(IWindsorInstaller[] installers)
        {
            var windsorContainer = CreateContainer();

            windsorContainer.Install(installers);

            return windsorContainer;
        }

        /// <summary>
        /// Creates a new Windsor container based on installers in current deployed directory
        /// </summary>
        public static IWindsorContainer Create()
        {
            var windsorContainer = CreateContainer();
            
            windsorContainer.Install(FromAssembly.This());

            return windsorContainer;
        }
    }
}
