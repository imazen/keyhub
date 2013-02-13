using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using KeyHub.Model;
using Microsoft.Web.WebPages.OAuth;
using KeyHub.Data;
using KeyHub.Web.Models;
using KeyHub.Web.ViewModels.User;
using WebMatrix.WebData;
using Membership = System.Web.Security.Membership;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public AccountController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of users
        /// </summary>
        /// <returns>User index view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
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
        public ActionResult DetailsPartial(int id)
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
                WebSecurity.CreateUserAndAccount(viewModel.User.UserName, viewModel.User.Password, new { Email = viewModel.User.Email });

                if (WebSecurity.Login(viewModel.User.UserName, viewModel.User.Password))
                {
                    if (Url.IsLocalUrl(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Failed to create a user with the provided username and password.");
            }

            //Viewmodel invalid, recall create
            return Create();
        }

        /// <summary>
        /// Edit a single User
        /// </summary>
        /// <param name="id">Id if the user to edit</param>
        /// <returns>Edit User view</returns>
        public ActionResult Edit(int id)
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
                    var user = context.Users.FirstOrDefault(x => x.UserId == viewModel.User.UserId);
                    if (user != null)
                    {
                        //Email can always be updated
                        user.Email = viewModel.User.Email;
                        context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "User not found");
                }
            }

            //Viewmodel invalid, recall edit
            return Edit(viewModel.User.UserId);
        }

        /// <summary>
        /// Login to KeyHub
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = (!String.IsNullOrEmpty(returnUrl)) ? returnUrl : Url.Action("Index", "Home", null, "http");
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
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
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
            WebSecurity.Logout();

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
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {Email = model.Email});

                if (WebSecurity.Login(model.UserName, model.Password))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Failed to create a user with the provided username and password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            var viewModel = new ChangePasswordViewModel(User.Identity.Name);
            return View(viewModel);
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(WebSecurity.ChangePassword(model.UserName, model.OldPassword, model.NewPassword))
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

        #region OpenAuth
        [AllowAnonymous]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if(OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: true))
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return Redirect(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                var loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalRegister", new ExternalUserCreateViewModel { Email = result.UserName, ExternalLoginData = loginData });
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalRegister(ExternalUserCreateViewModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (var db = new DataContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.Users.Add(new User { UserName = model.UserName, Email= model.Email });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: true);

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
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }
        #endregion

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
