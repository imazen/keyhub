using System.Web;
using Castle.MicroKernel.Registration;

namespace KeyHub.Web.Composition
{
    public class KeyHubApplicationAssemblyFilter : AssemblyFilter
    {
        public KeyHubApplicationAssemblyFilter()
            : base(HttpRuntime.BinDirectory, "KeyHub.*.dll")
        {
            
        }
    }
}
