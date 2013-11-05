using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    /// <summary>
    /// Hybrid scope accessor. 
    /// Tries a list of scope accessors until one returns a non-null scope.
    /// </summary>
    public class HybridScopeAccessor : IScopeAccessor {
        private readonly IEnumerable<IScopeAccessor> accessors;

        public HybridScopeAccessor(IEnumerable<IScopeAccessor> accessors) {
            this.accessors = accessors;
        }

        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context) {
            return accessors.Select(a => a.GetScope(context)).FirstOrDefault(s => s != null);
        }

        public void Dispose() {
            foreach (var a in accessors)
                a.Dispose();
        }
    }
}
