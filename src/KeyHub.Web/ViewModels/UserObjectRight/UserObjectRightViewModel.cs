using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;

namespace KeyHub.Web.ViewModels.UserObjectRight
{
    public class UserObjectRightViewModel : BaseViewModel<Model.UserObjectRight>
    {
        public UserObjectRightViewModel() {}

        public UserObjectRightViewModel(Model.UserObjectRight right)
            : this()
        {
            this.UserId = right.UserId;
            this.ObjectId = right.ObjectId;
            this.RightId = right.RightId;

            if (right is UserVendorRight)
                ObjectType = ObjectTypes.Vendor;
            else if (right is UserCustomerRight)
                ObjectType = ObjectTypes.Customer;
            else if (right is UserLicenseRight)
                ObjectType = ObjectTypes.License;
        }

        public override Model.UserObjectRight ToEntity(Model.UserObjectRight original)
        {
            if (original == null)
            {
                switch (ObjectType)
                {
                    case ObjectTypes.Vendor:
                        original = new UserVendorRight();
                        break;
                    case ObjectTypes.Customer:
                        original = new UserCustomerRight();
                        break;
                    case ObjectTypes.License:
                        original = new UserLicenseRight();
                        break;
                    default:
                        throw new ArgumentException("ViewModel object type is not supported");
                }
            }

            original.UserId = this.UserId;
            original.ObjectId = this.ObjectId;
            original.RightId = this.RightId;

            return original;
        }

        /// <summary>
        /// The UserId associated with this right entry
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }

        /// <summary>
        /// The ObjectId this right entry refers to
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The RightId this right entry refers to
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid RightId { get; set; }

        /// <summary>
        /// Type of UserObjectRight
        /// </summary>
        public ObjectTypes ObjectType { get; set; }
    }
}