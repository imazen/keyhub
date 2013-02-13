using System.Web.Mvc;
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
        private readonly IDataContextFactory dataContextFactory;
        public HomeController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public ActionResult Index()
        {
            var viewModel = new HomeViewModel();

            return View(viewModel);
        }

        /// <summary>
        /// Get the partial view for the main menu
        /// </summary>
        /// <param name="currentControllerName">Name of the currently active controller</param>
        /// <returns>Main menu partial view</returns>
        [AllowAnonymous]
        public ActionResult MainMenu(string currentControllerName)
        {
            var model = new MainMenuViewModel(currentControllerName);
            return PartialView(model);
        }
    }
}
