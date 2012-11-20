using System.ComponentModel.DataAnnotations;

namespace KeyHub.Web.Models
{
    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {}

        /// <summary>
        /// Viewmodel for changing a users password
        /// </summary>
        /// <param name="userName">Username of the user to change password for</param>
        public ChangePasswordViewModel(string userName)
        {
            UserName = userName;
        }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
