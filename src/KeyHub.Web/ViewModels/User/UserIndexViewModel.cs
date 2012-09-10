﻿using KeyHub.Data;
using KeyHub.Web.ViewModels;
using KeyHub.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Web.ViewModels.Right;

namespace KeyHub.Web.ViewModels.User
{
    /// <summary>
    /// Viewmodel for index list of Users
    /// </summary>
    public class UserIndexViewModel : BaseViewModel<Model.User>
    {
        public UserIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="users">List of users to show</param>
        public UserIndexViewModel(IEnumerable<Model.User> users)
            : this()
        {
            Users = new List<UserIndexViewItem>(
                from u in users select new UserIndexViewItem(u)
                );
        }

        /// <summary>
        /// List of users
        /// </summary>
        public List<UserIndexViewItem> Users { get; set; }

        /// <summary>
        /// Convert back to User instance
        /// </summary>
        /// <param name="original">Original User. If Null a new instance is created.</param>
        /// <returns>Not implemented</returns>
        public override Model.User ToEntity(Model.User original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// UserViewModel extension that includes the summary of roles
    /// </summary>
    public class UserIndexViewItem : UserViewModel
    {
        public UserIndexViewItem(Model.User user)
            : base(user)
        {
            RightsSummary = user.Rights.ToSummary(r => r.Right.DisplayName, maxItems:2, separator:", ");
        }

        /// <summary>
        /// Summary of user rights
        /// </summary>
        [DisplayName("Rights")]
        public string RightsSummary { get; set; }
    }
}