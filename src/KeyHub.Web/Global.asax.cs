using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using KeyHub.Core.Kernel;
using KeyHub.Web.Composition;

namespace KeyHub.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        protected void Application_Start()
        {
            container = CompositionContainerFactory.Create();
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(container));

            container.Resolve<IKernelContext>().RunKernelEvents(KernelEventsTypes.Startup);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_Stop()
        {
            container.Resolve<IKernelContext>().RunKernelEvents(KernelEventsTypes.Shutdown);
        }
    }
}