using Castle.MicroKernel.Registration;
using KeyHub.Core.Kernel;
using KeyHub.Web.Composition;

namespace KeyHub.Web.Installers
{
    public class KernelInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                    AllTypes.FromAssemblyInDirectory(new KeyHubApplicationAssemblyFilter())
                            .BasedOn<IKernelEvent>()
                            .WithService.FirstInterface()
                            .LifestyleTransient());

            container.Register(Component.For<IKernelContext>().ImplementedBy<KernelContext>().LifestyleSingleton());
        }
    }
}