using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using KeyHub.Core.Logging;
using KeyHub.Web.Logging;

namespace KeyHub.Web.Installers
{
    public class LoggingInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ILoggingService>().ImplementedBy<NLogLoggingService>().LifestyleSingleton());
        }
    }
}