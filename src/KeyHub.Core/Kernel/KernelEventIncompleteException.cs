using System;

namespace KeyHub.Core.Kernel
{
    /// <summary>
    /// Thrown when one or more kernel events procedures has failed to execute
    /// </summary>
    [Serializable]
    public class KernelEventIncompleteException : Exception
    {
        public KernelEventIncompleteException()
            : base("Some of the kernel events have failed! Check the log file for any specifics.") { }

        public KernelEventIncompleteException(Exception inner)
            : base("Some of the kernel events have failed! Check the log file for any specifics.", inner) { }

    }
}