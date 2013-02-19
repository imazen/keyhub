using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using KeyHub.Data.ApplicationIssues;

namespace KeyHub.Web.Installers
{
    public class WindsorUnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IApplicationIssueUnitOfWork>()
                         .ImplementedBy<ApplicationIssueUnitOfWork>()
                         .LifestyleTransient());
        }
    }
}