using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using KeyHub.Common.Collections;
using KeyHub.Core.Dependency;
using KeyHub.Core.Errors;
using KeyHub.Core.Logging;

namespace KeyHub.Core.Kernel
{
    /// <summary>
    /// Holds functionality for running Kernel events procedures
    /// </summary>
    public class KernelContext : IKernelContext
    {
        private readonly ILoggingService loggingService;
        private readonly IEnumerable<IKernelEvent> kernelEvents; 

        public KernelContext(ILoggingService loggingService, IEnumerable<IKernelEvent> kernelEvents)
        {
            this.loggingService = loggingService;
            this.kernelEvents = kernelEvents.ToList();
        }

        #region "Kernel events"

        /// <summary>
        /// Runs all Kernel events based on the given type in the correct sequence
        /// </summary>
        /// <param name="type">The Kernel type to run</param>
        public void RunKernelEvents(KernelEventsTypes type)
        {
            // Get the correct elements and run the events
            RunKernelEvents(kernelEvents.Where(x => x.EventType == type)
                                        .OrderBy(x => x.Priority));
        }

        /// <summary>
        /// Runs the given events in the order they are supplied
        /// </summary>
        /// <param name="events">Events to run</param>
        public void RunKernelEvents(params IKernelEvent[] events)
        {
            RunKernelEvents(events.AsEnumerable());
        }

        /// <summary>
        /// Runs all kernel events in the list
        /// </summary>
        /// <param name="kernelList">A list of kernel events</param>
        private void RunKernelEvents(IEnumerable<IKernelEvent> kernelList)
        {
            // Loop through all kernal events
            foreach (var kernelEvent in kernelList)
            {
                loggingService.Debug("Executing kernel event [{0}]", kernelEvent.DisplayName);

                KernelEventCompletedArguments kernelEventArguments = null;
                Exception kernelEventException = null;

                try
                {
                    kernelEventArguments = kernelEvent.Execute();
                }
                catch (Exception ex)
                {
                    kernelEventException = ex;
                }


                if (kernelEventException != null)
                {
                    // We got an exception during kernel event time, log this issue with Critical
                    var bootIssue = new GenericError
                                            {
                                                ErrorException = kernelEventException,
                                                ErrorMessage = "",
                                                Severity = ErrorSeverity.Critical
                                            };

                    loggingService.Fatal(bootIssue);

                    throw new KernelEventIncompleteException();
                }

                // Check if the kernel event returned a value
                if (kernelEventArguments == null)
                    kernelEventArguments = new KernelEventCompletedArguments
                    {
                        KernelEventSucceeded = false,
                        AllowContinue = false,
                        Issues = new List<IError>
                                        {
                                            new GenericError
                                            {
                                                ErrorException = null,
                                                ErrorMessage = "",
                                                Severity = ErrorSeverity.Critical
                                            }
                                        }.ToArray()
                    };

                if (!kernelEventArguments.KernelEventSucceeded) // Kernel event didn't succeed, log error
                    loggingService.Fatal(kernelEventArguments.Issues.ToArray());

                if (!kernelEventArguments.AllowContinue) // If we're not allowed to continue, throw exception to stop execution
                    throw new KernelEventIncompleteException();
            }
        }

        #endregion "Kernel events"
    }
}