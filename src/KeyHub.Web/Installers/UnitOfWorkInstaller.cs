using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using KeyHub.Data.ApplicationIssues;

namespace KeyHub.Web.Installers
{
    public class UnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IApplicationIssueUnitOfWork>()
                         .ImplementedBy<ApplicationIssueUnitOfWork>()
                         .LifestyleTransient());
        }
    }
}