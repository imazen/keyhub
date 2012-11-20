﻿using System.Collections.Generic;
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

            return new KernelEventCompletedArguments { AllowContinue = (!issueList.Any()), KernelEventSucceeded = (!issueList.Any()), Issues = issueList.ToArray() };
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