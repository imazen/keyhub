using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Web.ViewModels;

namespace KeyHub.Web.ViewModels.User
{
    /// <summary>
    /// Viewmodel for creating of a User
    /// </summary>
    public class UserCreateViewModel : BaseViewModel<Model.User>
    {
        public UserCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="user">User to edit</param>
        public UserCreateViewModel(bool thisOne)
        {
            User = new UserCreateViewItem(new Model.User());
        }

        /// <summary>
        /// Edited User
        /// </summary>
        public UserCreateViewItem User { get; set; }

        /// <summary>
        /// Convert back to User instance
        /// </summary>
        /// <returns>New User containing viewmodel data </returns>
        public Model.User ToEntity(Model.User original)
        {
            return User.ToEntity(null);
        }
    }

    /// <summary>
    /// UserEditViewItem extension that contains a list of SelectedRole Guids
    /// </summary>
    public class UserCreateViewItem : UserViewModel
    {
        public UserCreateViewItem()
            : base()
        { }

        public UserCreateViewItem(Model.User user)
            : base(user)
        {

        }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}