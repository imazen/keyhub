using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;

namespace KeyHub.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new DataContext();
            var iets = (from x in context.Vendors select new {NativeCountryName = x.Country.NativeCountryName + x.Country.CountryCode, 
                                                              VendorGuid = x.ObjectId})
                            .ToSelectList(x => x.VendorGuid, x => x.NativeCountryName);
            
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return View();
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
