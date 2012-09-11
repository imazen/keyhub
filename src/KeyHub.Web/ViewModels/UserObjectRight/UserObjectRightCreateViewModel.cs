using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;

namespace KeyHub.Web.ViewModels.UserObjectRight
{
    public class UserObjectRightCreateViewModel : UserObjectRightViewModel
    {
        public UserObjectRightCreateViewModel()
            : base()
        {
        }

        public UserObjectRightCreateViewModel(Model.UserObjectRight userObjectRight,
            ObjectTypes objectType, SelectList entityList)
            : base(userObjectRight)
        {
            if (userObjectRight.User == null)
                throw new ArgumentException("User in UserObjectRight cannot be null");

            this.ObjectType = objectType;
            this.UserName = userObjectRight.User.UserName;
            this.EntityList = entityList;
            this.RightName = userObjectRight.Right.DisplayName;
        }

        /// <summary>
        /// Name of the right
        /// </summary>
        [DisplayName("User")]
        public string UserName { get; set; }

        /// <summary>
        /// Name of the right
        /// </summary>
        [DisplayName("Right")]
        public string RightName { get; set; }

        /// <summary>
        /// List of entities (Vendors, Customers or Licenses) to asssign rights to
        /// </summary>
        public SelectList EntityList { get; set; }
    }
}