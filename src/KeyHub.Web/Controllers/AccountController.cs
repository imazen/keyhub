using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KeyHub.Data;
using KeyHub.Web.Models;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// Get list of users
        /// </summary>
        /// <returns>User index view</returns>
        public ActionResult Index()
        {
            using (var context = new DataContext(User.Identity))
            {
                // Eager loading users (except current user) and roles
                var usersQuery = (from u in context.Users where u.UserName != User.Identity.Name select u)
                                 .Include(u => u.Rights.Select(r => r.RightObject))
                                 .OrderBy(u => u.UserName);

                var viewModel = new UserIndexViewModel(usersQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="id">Id of the user to view</param>
        /// <returns>User index view</returns>
        public ActionResult DetailsPartial(Guid id)
        {
            using (var context = new DataContext())
            {
                var userQuery = (from u in context.Users where u.UserId == id select u);
                
                var viewModel = new UserViewModel(userQuery.FirstOrDefault());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single User
        /// </summary>
        /// <returns>Create User view</returns>
        public ActionResult Create()
        {
            using (var context = new DataContext())
            {
                var viewModel = new UserCreateViewModel(thisOne:true);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created User into context and redirect to index
        /// </summary>
        /// <param name="viewModel">Created UserViewModel</param>
        /// <returns>Redirectaction to index if successful</returns>
        [HttpPost]
        public ActionResult Create(UserCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                var user = Membership.CreateUser(viewModel.User.UserName,
                                      viewModel.User.Password,
                                      viewModel.User.Email,
                                      passwordQuestion: null,
                                      passwordAnswer: null,
                                      isApproved: true,
                                      providerUserKey: null,
                                      status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            //Viewmodel invalid, recall create
            return Create();
        }

        /// <summary>
        /// Edit a single User
        /// </summary>
        /// <param name="id">Id if the user to edit</param>
        /// <returns>Edit User view</returns>
        public ActionResult Edit(Guid id)
        {
            using (var context = new DataContext())
            {
                var userQuery = (from u in context.Users where u.UserId == id select u);
                
                var viewModel = new UserEditViewModel(userQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited User into context and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited UserViewModel</param>
        /// <returns>Redirectaction to index if successful</returns>
        [HttpPost]
        public ActionResult Edit(UserEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new DataContext())
                {
                    var user = System.Web.Security.Membership.GetUser(viewModel.User.UserName);
                    if (user != null)
                    {
                        //Email can always be updated
                        user.Email = viewModel.User.Email;
                        Membership.UpdateUser(user);

                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "User not found");
                }
            }

            //Viewmodel invalid, recall edit
            return Edit(viewModel.User.UserId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Register user and redirect to return URL if specified
        /// </summary>
        /// <param name="model">RegisterViewModel for new user</param>
        /// <param name="returnUrl">Url to redirect to after successfull registration</param>
        /// <returns>Redirect to Index, or ReturnUrl if specified</returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
