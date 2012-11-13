using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.Home
{
    /// <summary>
    /// Viewmodel for MainMenu
    /// </summary>
    public class MainMenuViewModel : BaseViewModel<Model.User>
    {
        public MainMenuViewModel(string controllerName)
            : base()
        {
            this.Controller = controllerName;
        }

        public override Model.User ToEntity(Model.User original)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// USername (loginname) for this user
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Controller { get; private set; }
    }
}