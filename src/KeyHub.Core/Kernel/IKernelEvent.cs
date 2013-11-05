namespace KeyHub.Core.Kernel
{
    /// <summary>
    /// Implement this to add classes to the specific kernal events
    /// </summary>
    public interface IKernelEvent
    {
        /// <summary>
        /// Executes the method for this class
        /// </summary>
        /// <returns>A <c>KernelEventCompletedArguments</c> object holding information about the event</returns>
        KernelEventCompletedArguments Execute();

        /// <summary>
        /// Gets the type for this event
        /// </summary>
        KernelEventsTypes EventType { get; }

        /// <summary>
        /// Gets the priority for this kernel event
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the display name of this event
        /// </summary>
        string DisplayName { get; }
    }
}