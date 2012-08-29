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
    /// Viewmodel for Homepage
    /// </summary>
    public class HomeViewModel : BaseViewModel<Model.User>
    {
        public HomeViewModel() : base() { }

        public override Model.User ToEntity(Model.User original)
        {
            throw new NotImplementedException();
        }
    }
}