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
    /// Viewmodel for editing of a User
    /// </summary>
    public class UserEditViewModel : BaseViewModel<Model.User>
    {
        public UserEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="user">User to edit</param>
        public UserEditViewModel(Model.User user)
        {
            User = new UserEditViewItem(user);
        }

        /// <summary>
        /// Edited User
        /// </summary>
        public UserEditViewItem User { get; set; }

        /// <summary>
        /// Convert back to User instance
        /// </summary>
        /// <returns>New User containing viewmodel data </returns>
        public override Model.User ToEntity(Model.User original)
        {
            return User.ToEntity(original);
        }
    }

    /// <summary>
    /// UserEditViewItem extension that contains a list of SelectedRole Guids
    /// </summary>
    public class UserEditViewItem : UserViewModel
    {
        public UserEditViewItem()
            : base()
        { }

        public UserEditViewItem(Model.User user)
            : base(user)
        {

        }
    }
}