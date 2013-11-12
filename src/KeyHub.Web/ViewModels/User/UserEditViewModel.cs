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
    public class UserEditViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}