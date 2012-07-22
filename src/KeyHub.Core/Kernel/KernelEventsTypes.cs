namespace KeyHub.Core.Kernel
{
    /// <summary>
    /// Represents a list of kernel events
    /// </summary>
    public enum KernelEventsTypes
    {
        /// <summary>
        /// Event that's raised when the application starts
        /// </summary>
        Startup,

        /// <summary>
        /// Events that's raised when the application shuts down
        /// </summary>
        Shutdown
    }
}