using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.User
{
    public class ExternalUserCreateViewModel
    {
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public string ExternalLoginData { get; set; }
    }
}