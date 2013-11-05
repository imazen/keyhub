using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the userrole for this application
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// Name of the System Administrator role
        /// </summary>
        public static readonly string SystemAdmin = "Sys_Administrator";

        /// <summary>
        /// Name of a regular user role
        /// </summary>
        public static readonly string RegularUser = "User";
    }
}
