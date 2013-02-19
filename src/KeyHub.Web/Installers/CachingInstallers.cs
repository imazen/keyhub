using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using KeyHub.BusinessLogic.Caching;
using KeyHub.Core.Caching;

namespace KeyHub.Web.Installers
{
    public class CachingInstallers : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<IPerRequestCacheProvider>().ImplementedBy<PerRequestCacheProvider>().LifestyleTransient());
            container.Register(Component.For<IRuntimeCacheProvider>().ImplementedBy<RuntimeCacheProvider>().LifestyleTransient());
        }
    }
}