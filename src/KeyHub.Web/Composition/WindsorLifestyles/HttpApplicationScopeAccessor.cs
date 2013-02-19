using System;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class HttpApplicationScopeAccessor: IScopeAccessor {
        private const string LifetimeScopeKey = "HttpApplicationScopeAccessor_LifetimeScope";

        private readonly Func<HttpContextBase> contextProvider = () => new HttpContextWrapper(HttpContext.Current);

        public HttpApplicationScopeAccessor(Func<HttpContextBase> contextProvider) {
            this.contextProvider = contextProvider;
        }

        public HttpApplicationScopeAccessor() {}

        private PerHttpApplicationLifestyleModule GetHttpModule() {
            var context = contextProvider();
            if (context == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerHttpApplicationLifestyle can only be used in ASP.NET");

            var app = context.ApplicationInstance;
            var lifestyleModule = app.Modules
                .Cast<string>()
                .Select(k => app.Modules[k])
                .OfType<PerHttpApplicationLifestyleModule>()
                .FirstOrDefault();

            if (lifestyleModule == null) {
                var message = string.Format("Looks like you forgot to register the http module {0}" +
                                               "\r\nAdd '<add name=\"PerHttpApplicationLifestyle\" type=\"{1}\" />' " +
                                               "to the <httpModules> section on your web.config",
                                               typeof(PerWebRequestLifestyleModule).FullName,
                                               typeof(PerWebRequestLifestyleModule).AssemblyQualifiedName);
                throw new Exception(message);
            }
            return lifestyleModule;
        }

        public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context) {
            return GetHttpModule().GetScope();
        }

        public void Dispose() {
            GetHttpModule().ClearScope();
        }
    }
}
