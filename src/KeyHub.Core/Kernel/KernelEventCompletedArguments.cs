using System.Collections.Generic;
using KeyHub.Core.Issues;

namespace KeyHub.Core.Kernel
{
    /// <summary>
    /// Holds information about the kernel event process
    /// </summary>
    public class KernelEventCompletedArguments
    {
        /// <summary>
        /// Gets or sets wether the kernel event has succesfully executed
        /// </summary>
        public bool KernelEventSucceeded { get; set; }

        /// <summary>
        /// Get or sets wether the application can continue after this event
        /// </summary>
        public bool AllowContinue { get; set; }

        /// <summary>
        /// Gets or sets the issue associated with the kernel event
        /// </summary>
        public IEnumerable<IIssue> Issues { get; set; }

        public KernelEventCompletedArguments()
        {
            Issues = new List<IIssue>();
        }
    }
}