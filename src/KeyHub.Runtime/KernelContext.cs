using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using KeyHub.Common.Collections;
using KeyHub.Core.Dependency;
using KeyHub.Core.Issues;
using KeyHub.Core.Kernel;

namespace KeyHub.Runtime
{
    /// <summary>
    /// Holds functionality for running Kernel events procedures
    /// </summary>
    public sealed class KernelContext
    {
        #region "Singleton"

        /// <summary>
        /// Gets the current instance of the ApplicationContext class
        /// </summary>
        public static KernelContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new KernelContext();
                    }
                }

                return instance;
            }
        }

        private static volatile KernelContext instance;
        private static object instanceLock = new object();

        private KernelContext()
        {
            // Create the boot elements collection to support lazy ordered binding
            kernelElements = new OrderingCollection<IKernelEvent, IGenericImportOrder>(lazyRule => lazyRule.Metadata.Order);

            // Compose this class using MEF
            InjectMef();
        }

        #endregion "Singleton"

        #region "MEF"

        /// <summary>
        /// Injects the ApplicationContext class with all Imports using MEF
        /// </summary>
        private void InjectMef()
        {
            // Compose this class using MEF
            DependencyContext.Instance.Compose(this);
        }

        #endregion "MEF"

        #region "Kernel elements"

        /// <summary>
        /// Gets or sets the current list of boot elements
        /// </summary>
        [ImportMany]
        private OrderingCollection<IKernelEvent, IGenericImportOrder> kernelElements { get; set; }

        #endregion "Kernel elements"

        #region "Kernel events"

        /// <summary>
        /// Runs all Kernel events based on the given type in the correct sequence
        /// </summary>
        /// <param name="type">The Kernel type to run</param>
        public void RunKernelEvents(Core.Kernel.KernelEventsTypes type)
        {
            // Get the correct elements and run the events
            RunKernelEvents(kernelElements.Where(x => x.Value.EventType == type)
                                          .OrderBy(x => x.Metadata.Order)
                                          .Select(x => x.Value));
        }

        /// <summary>
        /// Runs the given events in the order they are supplied
        /// </summary>
        /// <param name="events">Events to run</param>
        public void RunKernelEvents(params IKernelEvent[] events)
        {
            RunKernelEvents(events);
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
                Runtime.LogContext.Instance.Debug("Executing kernal event [{0}]", kernelEvent.DisplayName);

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
                    IIssue bootIssue = new GenericIssue()
                    {
                        IssueException = kernelEventException,
                        IssueMessage = "",
                        Severity = IssueSeverity.Critical
                    };

                    Runtime.LogContext.Instance.Fatal(bootIssue);

                    throw new KernelEventIncompleteException();
                }

                // Check if the kernel event returned a value
                if (kernelEventArguments == null)
                    kernelEventArguments = new KernelEventCompletedArguments()
                    {
                        KernelEventSucceeded = false,
                        AllowContinue = false,
                        Issues = new List<IIssue>
                                        {
                                            new GenericIssue()
                                            {
                                                IssueException = kernelEventException,
                                                IssueMessage = "",
                                                Severity = IssueSeverity.Critical
                                            }
                                        }.ToArray()
                    };

                if (!kernelEventArguments.KernelEventSucceeded) // Kernel event didn't succeed, log error
                    Runtime.LogContext.Instance.Fatal(kernelEventArguments.Issues.ToArray());

                if (!kernelEventArguments.AllowContinue) // If we're not allowed to continue, throw exception to stop execution
                    throw new KernelEventIncompleteException();
            }
        }

        #endregion "Kernel events"
    }
}