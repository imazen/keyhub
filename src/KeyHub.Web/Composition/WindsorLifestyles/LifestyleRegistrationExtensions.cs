using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public static class LifestyleRegistrationExtensions {
        /// <summary>
        /// One component instance per web session.
        /// Warning: because the session end event request only works InProc, components can't be reliably disposed. Burden is also affected.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> PerWebSession<S>(this LifestyleGroup<S> @group) where S : class {
            return @group.Scoped<WebSessionScopeAccessor>();
        }

        /// <summary>
        /// One component instance per HttpApplication instance.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> PerHttpApplication<S>(this LifestyleGroup<S> @group) where S : class {
            return @group.Scoped<HttpApplicationScopeAccessor>();
        }

        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, transient.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestTransient<S>(this LifestyleGroup<S> @group) where S : class {
            return @group.Scoped<HybridPerWebRequestTransientScopeAccessor>();
        }

        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, one per thread.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestPerThread<S>(this LifestyleGroup<S> @group) where S : class {
            return @group.Scoped<HybridPerWebRequestPerThreadScopeAccessor>();
        }
    }
}