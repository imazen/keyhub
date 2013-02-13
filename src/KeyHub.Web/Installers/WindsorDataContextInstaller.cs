using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using KeyHub.Data;

namespace KeyHub.Web.Installers
{
    public class WindsorDataContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                Component.For<IIdentity>()
                         .LifeStyle.PerWebRequest
                         .UsingFactoryMethod(() => HttpContext.Current.User.Identity));

            container.Register(
                Component.For<IDataContext>()
                         .ImplementedBy<DataContext>()
                         .LifestyleTransient());

            container.Register(
                Component.For<IDataContextByUser>()
                         .ImplementedBy<DataContextByUser>()
                         .LifestyleTransient());
            
            container.Register(
                Component.For<IDataContextByTransaction>()
                         .ImplementedBy<DataContextByTransaction>()
                         .LifestyleTransient());

            container.Register(Component.For<IDataContextFactory>().AsFactory());
        }
    }
}