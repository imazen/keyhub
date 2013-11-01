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
            //
            // Entity Framework sometimes becomes upset that the model doesn't match its
            // record of the schema, even when there is no difference.  To fix this, 
            // we're calling SetInitializer(null) per https://digitaltoolfactory.net/blog/2012/08/how-to-fix-the-model-backing-the-context-has-changed-since-the-database-was-created-error/
            //
            // I do not know why this is not needed for DataContext though it is needed
            // for DataContextByUser.
            //
            Database.SetInitializer<KeyHub.Data.DataContextByUser>(null);
            Database.SetInitializer<KeyHub.Data.DataContextByTransaction>(null);

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