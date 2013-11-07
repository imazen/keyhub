using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data
{
    public class DataContextByUser : AbstractFilteredDataContext, IDataContextByUser
    {
        private User User;

        protected override IEnumerable<Guid> ResolveAuthorizedVendors()
        {
            if (User.IsSystemAdmin)
                return (from x in this.Set<Vendor>() select x.ObjectId).ToList();
            else
                return (from r in User.Rights where r is UserVendorRight && r.RightId == VendorAdmin.Id select r.ObjectId).ToList();
        }

        protected override bool ContextIsForSystemAdmin()
        {
            return User.IsSystemAdmin;
        }

        protected override bool ContextIsForVendorAdmin()
        {
            return User.IsVendorAdmin;
        }

        protected override ICollection<UserObjectRight> GetNonvendorUserObjectRights()
        {
            return User.Rights;
        }


        /// <summary>
        /// Gets a datacontext based on a provided userIdentity
        /// </summary>
        /// <param name="userIdentity">Current user identity</param>
        /// <returns>
        /// A context based on the provided user idenity. If user is unknown or anonymous 
        /// a datacontext is still returned but will have empty collections
        /// </returns>
        public DataContextByUser(IIdentity userIdentity)
        {
            User = this.GetUser(userIdentity);

            ApplyFilters();
        }

        #region "Resolving user rights"

        #endregion
    }
}
