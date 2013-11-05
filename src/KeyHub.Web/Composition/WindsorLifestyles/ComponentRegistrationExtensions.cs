using Castle.MicroKernel.Registration;

namespace KeyHub.Web.Composition.WindsorLifestyles
{
    public static class ComponentRegistrationExtensions
    {
        public static ComponentRegistration<TService> LifestylePerSession<TService>(this ComponentRegistration<TService> reg)
            where TService : class
        {
            return reg.LifestyleScoped<WebSessionScopeAccessor>();
        }
    }
}