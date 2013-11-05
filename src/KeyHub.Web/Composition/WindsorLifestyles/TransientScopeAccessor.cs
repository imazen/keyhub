using Castle.MicroKernel.Lifestyle.Scoped;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class TransientScopeAccessor: IScopeAccessor {
        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context) {
            return new DefaultLifetimeScope();
        }

        public void Dispose() {
        }
    }
}
