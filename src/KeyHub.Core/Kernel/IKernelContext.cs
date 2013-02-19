namespace KeyHub.Core.Kernel
{
    public interface IKernelContext
    {
        /// <summary>
        /// Runs all Kernel events based on the given type in the correct sequence
        /// </summary>
        /// <param name="type">The Kernel type to run</param>
        void RunKernelEvents(Core.Kernel.KernelEventsTypes type);

        /// <summary>
        /// Runs the given events in the order they are supplied
        /// </summary>
        /// <param name="events">Events to run</param>
        void RunKernelEvents(params IKernelEvent[] events);
    }
}