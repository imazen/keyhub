using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using KeyHub.Data.BusinessRules;
using KeyHub.Web.Composition;

namespace KeyHub.Web.Installers
{
    public class BusinessRulesInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                    AllTypes.FromAssemblyInDirectory(new KeyHubApplicationAssemblyFilter())
                            .BasedOn<IBusinessRule>()
                            .WithService.AllInterfaces()
                            .LifestyleTransient());

            container.Register(
                Component.For<IBusinessRuleExecutor>().ImplementedBy<BusinessRuleExecutor>().LifestyleTransient());

            container.Register(Component.For<IBusinessRuleExecutorFactory>().AsFactory());
        }
    }
}