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
    /// Viewmodel for MainMenu
    /// </summary>
    public class MainMenuViewModel : BaseViewModel<Model.User>
    {
        public MainMenuViewModel(Model.User currentUser, string controllerName)
            : base()
        {
            CurrentUser = new CurrentUserViewModel(currentUser);

            this.Controller = controllerName;
        }

        public override Model.User ToEntity(Model.User original)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }

        /// <summary>
        /// USername (loginname) for this user
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Controller { get; private set; }
    }
}