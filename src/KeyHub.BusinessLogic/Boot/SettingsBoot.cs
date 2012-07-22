using System.ComponentModel.Composition;
using KeyHub.Core.Kernel;

namespace KeyHub.BusinessLogic.Boot
{
    /// <summary>
    /// Initializes all settings classes supported by the Framework
    /// </summary>
    [Export(typeof(IKernelEvent))]
    [ExportMetadata("Order", 2)]
    public class SettingsBoot : IKernelEvent
    {
        /// <summary>
        /// Boots the FaceBoot procedure
        /// </summary>
        /// <returns></returns>
        public KernelEventCompletedArguments Execute()
        {
            // Triggers creation of the singleton class
            var settings = Runtime.SettingsContext.Instance;

            // Return empty arguments
            return new KernelEventCompletedArguments() { AllowContinue = true, KernelEventSucceeded = true };
        }

        /// <summary>
        /// Gets the display name for the Settings boot procedure
        /// </summary>
        public string DisplayName
        {
            get { return "Settings boot"; }
        }

        public KernelEventsTypes EventType
        {
            get { return KernelEventsTypes.Startup; }
        }
    }
}