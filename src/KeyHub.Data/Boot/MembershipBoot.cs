using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Security;
using KeyHub.Core.Issues;
using KeyHub.Core.Kernel;

namespace KeyHub.Data.Boot
{
    /// <summary>
    /// Holds the boot procedure for the Membership provider.
    /// This boot procedure must run after the RolesBoot class.
    /// </summary>
    [Export(typeof(IKernelEvent))]
    [ExportMetadata("Order", 101)]
    public class MembershipBoot : IKernelEvent
    {
        private List<IIssue> issueList = new List<IIssue>();

        /// <summary>
        /// Boots the MembershipBoot procedure
        /// </summary>
        /// <returns></returns>
        public KernelEventCompletedArguments Execute()
        {
            // Create administator Role if not already present
            var currentRoles = System.Web.Security.Roles.GetAllRoles();
            if (!currentRoles.Contains("Sys_Administrator"))
                System.Web.Security.Roles.CreateRole("Sys_Administrator");

            // Create an administator of not already present
            var administrator = System.Web.Security.Membership.GetUser("admin");
            if (administrator == null)
            {
                // Create administrator user
                administrator = CreateUser("admin",
                                           "password",
                                           "websites@lucrasoft.nl");

                // Add users to roles if they exist
                if (administrator != null)
                    System.Web.Security.Roles.AddUserToRole("admin", "Sys_Administrator");
            }

            return new KernelEventCompletedArguments() { AllowContinue = (issueList.Count() == 0), KernelEventSucceeded = (issueList.Count() == 0), Issues = issueList.ToArray() };
        }

        private MembershipUser CreateUser(string userName, string password, string email)
        {
            // Create user
            System.Web.Security.MembershipCreateStatus status;
            var user = System.Web.Security.Membership.CreateUser(userName,
                                                                 password,
                                                                 email,
                                                                 null,
                                                                 null,
                                                                 true,
                                                                 out status);

            // Add issue if the status is not success
            if (status != System.Web.Security.MembershipCreateStatus.Success)
                issueList.Add(new GenericIssue(null, status.ToString(), IssueSeverity.Error));

            return user;
        }

        /// <summary>
        /// Gets the display name for the Membership boot procedure
        /// </summary>
        public string DisplayName
        {
            get { return "Membership boot"; }
        }

        public KernelEventsTypes EventType
        {
            get { return KernelEventsTypes.Startup; }
        }
    }
}