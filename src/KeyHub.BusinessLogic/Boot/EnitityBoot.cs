using System.ComponentModel.Composition;
using System.Data.Entity;
using KeyHub.Core.Kernel;
using KeyHub.Data;
using KeyHub.Data.Initializers;

namespace KeyHub.BusinessLogic.Boot
{
    /// <summary>
    /// Boots the Entity classes and created the database if neccesary.
    /// </summary>
    [Export(typeof(IKernelEvent))]
    [ExportMetadata("Order", 1)]
    public class EnitityBoot : IKernelEvent
    {
        public KernelEventCompletedArguments Execute()
        {
            // Initializes and seeds the database.
            Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChangesInitializer());

            // Forces initialization of database on model changes.
            using (var context = new DataContext())
            {
                context.Database.Initialize(true);
            }

            return new KernelEventCompletedArguments() { AllowContinue = true, KernelEventSucceeded = true, Issues = null };
        }

        public string DisplayName
        {
            get { return "Entity data boot"; }
        }

        public KernelEventsTypes EventType
        {
            get { return KernelEventsTypes.Startup; }
        }
    }
}