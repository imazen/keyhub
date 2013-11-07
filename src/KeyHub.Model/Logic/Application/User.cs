using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the user for this application
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Check if the current user is system administrator
        /// </summary>
        public virtual bool IsSystemAdmin
        {
            get
            {
                if (UserName == null)
                    return false;

                var userRoles = System.Web.Security.Roles.GetRolesForUser(UserName);
                return userRoles.Contains(Role.SystemAdmin);
            }
        }

        /// <summary>
        /// Check if the current user is VendorAdmin
        /// </summary>
        public virtual bool IsVendorAdmin
        {
            get
            {
                var right = (from r in this.Rights where r is UserVendorRight && r.RightId == VendorAdmin.Id select r).FirstOrDefault();
                return (right != null);
            }
        }

        /// <summary>
        /// Check if the current user can edit curstomer info
        /// </summary>
        public bool CanEditCustomerInfo
        {
            get
            {
                if (!IsVendorAdmin && !IsSystemAdmin)
                {
                    var right = (from r in this.Rights where r is UserCustomerRight && r.RightId == EditEntityMembers.Id select r).FirstOrDefault();
                    return (right != null);
                }
                else
                {
                    return IsSystemAdmin | IsVendorAdmin;
                }
            }
        }

        /// <summary>
        /// Check if the current user can edit license info
        /// </summary>
        public bool CanEditLicenseInfo
        {
            get
            {
                if (!IsVendorAdmin && !CanEditCustomerInfo && !IsSystemAdmin)
                {
                    var right = (from r in this.Rights where r is UserLicenseRight && r.RightId == EditLicenseInfo.Id select r).FirstOrDefault();
                    return (right != null);
                }
                else
                {
                    return IsSystemAdmin | IsVendorAdmin | CanEditCustomerInfo;
                }
            }
        }
    }
}