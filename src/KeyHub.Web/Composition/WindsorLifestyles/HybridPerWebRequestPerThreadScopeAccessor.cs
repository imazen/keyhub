using Castle.MicroKernel.Lifestyle;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class HybridPerWebRequestPerThreadScopeAccessor: HybridPerWebRequestScopeAccessor {
        public HybridPerWebRequestPerThreadScopeAccessor() :
            base(new ThreadScopeAccessor()) { }
    }
}
