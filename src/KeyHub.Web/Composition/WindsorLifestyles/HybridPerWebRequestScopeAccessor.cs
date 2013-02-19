using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Web;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class HybridPerWebRequestScopeAccessor: IScopeAccessor {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridPerWebRequestScopeAccessor(IScopeAccessor secondaryScopeAccessor) {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context) {
            if (HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized)
                return webRequestScopeAccessor.GetScope(context);
            return secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose() {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }
    }
}
