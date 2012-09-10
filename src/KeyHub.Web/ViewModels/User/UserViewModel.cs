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
    public class UserViewModel : BaseViewModel<Model.User>
    {
        public UserViewModel(){ }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="user">User that this viewmodel represents</param>
        public UserViewModel(Model.User user):this()
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.LastActivityDate = user.LastActivityDate;

            if (this.UserName == null) return;

            var membershipUser = System.Web.Security.Membership.GetUser(user.UserName);
            if (membershipUser != null)
            {
                this.Email = membershipUser.Email;
            }
        }

        /// <summary>
        /// Convert back to User instance
        /// </summary>
        /// <param name="original">Original User. If Null a new instance is created.</param>
        /// <returns>User containing viewmodel data </returns>
        public override Model.User ToEntity(Model.User original)
        {
            Model.User current = original ?? new Model.User();

            current.UserId = this.UserId;
            current.UserName = this.UserName;
            current.LastActivityDate = this.LastActivityDate;

            return current;
        }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid UserId { get; set; }

        /// <summary>
        /// USername (loginname) for this user
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Last activity date
        /// </summary>
        [Required]
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}