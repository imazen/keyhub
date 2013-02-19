using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Security;
using KeyHub.Core.Issues;
using KeyHub.Core.Kernel;
using KeyHub.Model;
using WebMatrix.WebData;

namespace KeyHub.Data.Boot
{
    /// <summary>
    /// Holds the boot procedure for the Membership provider.
    /// This boot procedure must run after the RolesBoot class.
    /// </summary>
    public class MembershipBoot : IKernelEvent
    {
        private List<IIssue> issueList = new List<IIssue>();

        public KernelEventCompletedArguments Execute()
        {
            WebSecurity.InitializeDatabaseConnection("DataContext", "Users", "UserId", "UserName", autoCreateTables:true);

            // Create administator Role if not already present
            var currentRoles = Roles.GetAllRoles();
            if (!currentRoles.Contains(Role.SystemAdmin))
                Roles.CreateRole(Role.SystemAdmin);
            
            if (!currentRoles.Contains(Role.RegularUser))
                Roles.CreateRole(Role.RegularUser);

            // Create an administator of not already present        
            if (!WebSecurity.UserExists("admin"))
            {
                // Create administrator user
                WebSecurity.CreateUserAndAccount("admin", "password", new { Email = "websites@lucrasoft.nl" });
                Roles.AddUserToRole("admin", Role.SystemAdmin);
            }
            if (!WebSecurity.UserExists("fleppie"))
            {
                // Create administrator user
                WebSecurity.CreateUserAndAccount("fleppie", "test", new { Email = "floris@lucrasoft.nl" });
                Roles.AddUserToRole("fleppie", Role.SystemAdmin);
            }

            // Create an imazen account
            if (!WebSecurity.UserExists("imazen"))
            {
                // Create administrator user
                WebSecurity.CreateUserAndAccount("imazen", "nathanael", new { Email = "nathanael.jones@gmail.com" });
                Roles.AddUserToRole("imazen", Role.SystemAdmin);
            }

            return new KernelEventCompletedArguments { AllowContinue = (!issueList.Any()), KernelEventSucceeded = (!issueList.Any()), Issues = issueList.ToArray() };
        }

        public string DisplayName
        {
            get { return "Membership boot"; }
        }

        public KernelEventsTypes EventType
        {
            get { return KernelEventsTypes.Startup; }
        }

        public int Priority
        {
            get { return 101; }
        }
    }
}