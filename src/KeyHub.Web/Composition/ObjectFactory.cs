using Castle.Windsor;
using System.Collections.Generic;

namespace KeyHub.Web.Composition
{
    /// <summary>
    /// ObjectFactoryBase for default injection of dependencies
    /// </summary>
    public class ObjectFactory
    {
        public ObjectFactory()
        {
            WindsorContainer = CompositionContainerFactory.Create();
        }

        #region "IObjectFactory members"

        public object Resolve(System.Type type)
        {
            return WindsorContainer.Resolve(type);
        }

        public T Resolve<T>()
        {
            return WindsorContainer.Resolve<T>();
        }

        public T Resolve<T>(string contractName)
        {
            return WindsorContainer.Resolve<T>(contractName);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return WindsorContainer.ResolveAll<T>();
        }

        #endregion

        public IWindsorContainer WindsorContainer { get; set; }
        
        public void Dispose()
        {
            if (WindsorContainer != null)
                WindsorContainer.Dispose();
        }
    }
}