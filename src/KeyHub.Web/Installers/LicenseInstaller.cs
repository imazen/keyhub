using Castle.MicroKernel.Registration;
using KeyHub.BusinessLogic.LicenseValidation;

namespace KeyHub.Web.Installers
{
    public class LicenseInstaller  : IWindsorInstaller 
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ILicenseValidator>().ImplementedBy<LicenseValidator>().LifestyleTransient());
        }
    }
}