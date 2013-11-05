using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using KeyHub.Core.Mail;
using KeyHub.Web.Mail;

namespace KeyHub.Web.Installers
{
    public class MailServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMailService>()
                         .ImplementedBy<MailService>()
                         .LifestyleTransient());
        }
    }
}