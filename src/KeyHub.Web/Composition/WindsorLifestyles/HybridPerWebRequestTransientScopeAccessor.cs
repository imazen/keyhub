namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class HybridPerWebRequestTransientScopeAccessor : HybridPerWebRequestScopeAccessor {
        public HybridPerWebRequestTransientScopeAccessor() : 
            base(new TransientScopeAccessor()) {}
    }
}
