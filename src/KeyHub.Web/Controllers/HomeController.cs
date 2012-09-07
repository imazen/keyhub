using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Data;
using KeyHub.Web.ViewModels.Home;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();

            return View(viewModel);
        }

        /// <summary>
        /// Get the partial view for the main menu
        /// </summary>
        /// <returns>Main menu partial view</returns>
        [AllowAnonymous]
        public ActionResult MainMenu()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var model = new CurrentUserViewModel(context.GetUserByIdentity(User.Identity));
                return PartialView(model);
            }
        }
    }
}
