using System.Web.Http.Controllers;
using System.Web.Mvc;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using KeyHub.Data;

namespace KeyHub.Web.Installers
{
    public class WindsorControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient());
            container.Register(FindApiControllers().Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }

        /// <summary>
        /// Find API controllers within this assembly
        /// </summary>
        /// <returns></returns>
        private BasedOnDescriptor FindApiControllers()
        {
            return AllTypes.FromThisAssembly()
                .BasedOn<IHttpController>();
        }
    }
}