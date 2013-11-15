using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.ViewModels.Home
{
    /// <summary>
    /// Viewmodel for Homepage
    /// </summary>
    public class HomeViewModel : BaseViewModel<Model.User>
    {
        public HomeViewModel() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser">Current user</param>
        public HomeViewModel(Model.User currentUser) : base()
        {
            CurrentUser = new CurrentUserViewModel(currentUser);
        }

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }
    }
}