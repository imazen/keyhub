using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.WebPages.OAuth;

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
            this.Email = user.Email;
            this.HasLocalAccount = OAuthWebSecurity.HasLocalAccount(user.UserId);
        }

        /// <summary>
        /// Convert back to User instance
        /// </summary>
        /// <param name="original">Original User. If Null a new instance is created.</param>
        /// <returns>User containing viewmodel data </returns>
        public Model.User ToEntity(Model.User original)
        {
            Model.User current = original ?? new Model.User();

            current.UserId = this.UserId;
            current.Email = this.Email;

            return current;
        }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        /// <summary>
        /// Indicates if the user has a local acount. If <b>False</b> then OpenAuth account.
        /// </summary>
        public Boolean HasLocalAccount { get; set; }
    }
}