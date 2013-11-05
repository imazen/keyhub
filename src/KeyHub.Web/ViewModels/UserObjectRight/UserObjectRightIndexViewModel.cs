using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Model;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.ViewModels.UserObjectRight
{
    /// <summary>
    /// ViewModel for index list of rights
    /// </summary>
    public class UserObjectRightIndexViewModel : BaseViewModel<Model.UserObjectRight>
    {
        public UserObjectRightIndexViewModel()
        {
        }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="currentUser">Current user</param>
        /// <param name="userId">Id of the user rights belong to</param>
        /// <param name="rights">List of UserObjectRights to show</param>
        public UserObjectRightIndexViewModel(Model.User currentUser, int userId, IEnumerable<Model.UserObjectRight> rights)
        {
            CurrentUser = new CurrentUserViewModel(currentUser);
            UserId = userId;
            Rights = new List<UserObjectRightIndexViewItem>(
                from r in rights select new UserObjectRightIndexViewItem(r)
                );
        }

        /// <summary>
        /// Id of the user rights are being shown for
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// List of UserObjectRightViewItems
        /// </summary>
        public IEnumerable<UserObjectRightIndexViewItem> Rights;

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }

        /// <summary>
        /// ToEntity is not implemented
        /// </summary>
        /// <param name="original"></param>
        /// <returns>NotImplementedException</returns>
        public override Model.UserObjectRight ToEntity(Model.UserObjectRight original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// ViewModel for single index list item
    /// </summary>
    public class UserObjectRightIndexViewItem : UserObjectRightViewModel
    {
        public UserObjectRightIndexViewItem() : base() {}

        public UserObjectRightIndexViewItem(Model.UserObjectRight right)
            : base(right)
        {
            RightName = right.Right.DisplayName;

            ObjectName = GetEntityName(right);
        }

        /// <summary>
        /// Name of the right
        /// </summary>
        [DisplayName("Right")]
        public string RightName { get; set; }

        /// <summary>
        /// Entity name the right is assiciated to
        /// </summary>
        [DisplayName("Entity")]
        public string ObjectName { get; set; }

        /// <summary>
        /// Resolve display name of associated entity
        /// </summary>
        /// <param name="right">Right to show entity name of</param>
        /// <returns>Name of the associated entity</returns>
        private string GetEntityName(Model.UserObjectRight right)
        {
            if (right is Model.UserVendorRight)
            {
                var vendorRight = right as Model.UserVendorRight;
                return (vendorRight.Vendor != null) ? vendorRight.Vendor.Name : "";
            }
            else if (right is Model.UserCustomerRight)
            {
                var customerRight = right as Model.UserCustomerRight;
                return (customerRight.Customer != null) ? customerRight.Customer.Name : "";
            }
            else if (right is Model.UserLicenseRight)
            {
                //var licenseRight = right as Model.UserCustomerRight;
                //return (customerRight.Customer != null) ? customerRight.Customer.Name : "";

                return ((Model.UserLicenseRight)right).License.Sku.SkuCode;
            }
            throw new NotSupportedException(string.Format("UserObjectRight of type '{0}' is not supported as UserObjectRightViewItem", right.GetType()));
        }
    }
}