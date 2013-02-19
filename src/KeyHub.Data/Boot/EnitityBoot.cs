using System.ComponentModel.Composition;
using System.Data.Entity;
using KeyHub.Core.Kernel;
using KeyHub.Data;
using KeyHub.Data.Migrations;
using System.Data.Entity.Infrastructure;
using KeyHub.Core.Issues;

namespace KeyHub.Data.Boot
{
    /// <summary>
    /// Boots the Entity classes and created the database if neccesary.
    /// </summary>
    public class EnitityBoot : IKernelEvent
    {
        public KernelEventCompletedArguments Execute()
        {
            // Hhack to disable migrating when visual studio creates migrations.
            // This is due to the fact that EntityFramework uses some static information and this 
            // application loads everyting upfront
            if (!System.AppDomain.CurrentDomain.SetupInformation.AppDomainManagerAssembly.Contains(
                "Microsoft.VisualStudio.Platform.AppDomainManager"))
            {
                var configuration = new MigrationConfiguration();
                var migrator = new DbSeederMigrator<DataContext>(configuration);

                try
                {
                    migrator.MigrateToLatestVersion();
                }
                catch (System.Exception ex)
                {
                    return new KernelEventCompletedArguments
                    {
                        AllowContinue = false,
                        KernelEventSucceeded = false,
                        Issues = new IIssue[] 
                        { 
                            new GenericIssue
                            {
                                IssueMessage = "Could not migrate to latest version.",
                                IssueException = ex,
                                Severity = IssueSeverity.Critical
                            }
                        }
                    };
                }
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
        
        public int Priority
        {
            get { return 1; }
        }
    }
}