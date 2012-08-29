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
        public bool IsVendorAdmin
        {
            get
            {
                var right = (from r in this.Rights where r is UserVendorRight && r.RightId == VendorAdmin.Id select r).FirstOrDefault();
                return (right != null);
            }
        }

        public bool CanEditCustomerInfo
        {
            get
            {
                if (!IsVendorAdmin)
                {
                    var right = (from r in this.Rights where r is UserCustomerRight && r.RightId == EditEntityMembers.Id select r).FirstOrDefault();
                    return (right != null);
                }
                else
                {
                    return IsVendorAdmin;
                }
            }
        }

        public bool CanEditLicenseInfo
        {
            get
            {
                if (!IsVendorAdmin && !CanEditCustomerInfo)
                {
                    var right = (from r in this.Rights where r is UserLicenseRight && r.RightId == EditLicenseInfo.Id select r).FirstOrDefault();
                    return (right != null);
                }
                else
                {
                    return IsVendorAdmin | CanEditCustomerInfo;
                }
            }
        }
    }
}