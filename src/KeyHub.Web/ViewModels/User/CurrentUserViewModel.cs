using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;

namespace KeyHub.Web.ViewModels.User
{
    /// <summary>
    /// Viewmodel for a single user 
    /// </summary>
    public class CurrentUserViewModel
    {
        public CurrentUserViewModel()
            :base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="user">User that this viewmodel represents</param>
        public CurrentUserViewModel(Model.User user)
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.IsSystemAdmin = user.IsSystemAdmin;
            this.IsVendorAdmin = user.IsVendorAdmin;
            this.CanEditCustomerInfo = user.CanEditCustomerInfo;
            this.CanEditLicenseInfo = user.CanEditLicenseInfo;
        }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; private set; }

        /// <summary>
        /// Username (loginname) for this user
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; private set; }

        /// <summary>
        /// Email of this user
        /// </summary>
        [Required]
        public string Email { get; private set; }

        /// <summary>
        /// Check if user is system administrator
        /// </summary>
        public bool IsSystemAdmin { get; private set; }

        /// <summary>
        /// Check if user is vendor administrator
        /// </summary>
        public bool IsVendorAdmin { get; private set; }

        /// <summary>
        /// Check if user can edit customer info
        /// </summary>
        public bool CanEditCustomerInfo { get; private set; }

        /// <summary>
        /// Check if user can edit license info
        /// </summary>
        public bool CanEditLicenseInfo { get; private set; }
    }
}