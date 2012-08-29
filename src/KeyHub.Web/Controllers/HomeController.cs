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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();

            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var model = new CurrentUserViewModel(context.GetUserByIdentity(User.Identity));
                return PartialView(model);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
