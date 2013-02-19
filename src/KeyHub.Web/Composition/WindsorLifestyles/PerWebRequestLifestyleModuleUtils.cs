using System.Reflection;
using Castle.MicroKernel.Lifestyle;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    public class PerWebRequestLifestyleModuleUtils {

        private static readonly FieldInfo InitializedFieldInfo = typeof(PerWebRequestLifestyleModule).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

        public static bool IsInitialized {
            get {
                return (bool)InitializedFieldInfo.GetValue(null);
            }
        }
    }
}
