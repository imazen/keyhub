using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for Vendor entity
    /// </summary>
    public class VendorController : Controller
    {
        /// <summary>
        /// Get list of vendors
        /// </summary>
        /// <returns>Vendor index list view</returns>
        public ActionResult Index()
        {
            DataContext context = new DataContext();

            return View(context.Vendors.ToList());
        }

        /// <summary>
        /// View a single vendor's details
        /// </summary>
        /// <param name="id">GUID of vendor</param>
        /// <returns>Vendor details view</returns>
        public ActionResult Details(Guid id)
        {
            DataContext context = new DataContext();

            Vendor vendor = (from v in context.Vendors where v.ObjectId == id select v).FirstOrDefault();

            return View(vendor);
        }

        /// <summary>
        /// Edit a single vendor
        /// </summary>
        /// <param name="id">GUID of vendor to edit</param>
        /// <returns>Vendor edit view</returns>
        public ActionResult Edit(Guid id)
        {
            DataContext context = new DataContext();

            Vendor vendor = (from v in context.Vendors where v.ObjectId == id select v).FirstOrDefault();

            VendorViewModel viewModel = new VendorViewModel();
            //viewModel.FillFromEntity(vendor);

            return View(viewModel);
        }

        /// <summary>
        /// Edit a single vendor
        /// </summary>
        /// <param name="id">GUID of vendor to edit</param>
        /// <returns>Vendor edit view</returns>
        public ActionResult Insert(VendorViewModel vendorViewModel)
        {
            DataContext context = new DataContext();
            var domainVendor = context.Vendors.Find(vendorViewModel.Vendor.ObjectId);

            domainVendor = vendorViewModel.ToEntity(domainVendor);

            
            context.Vendors.Add(domainVendor);

            //Vendor vendor = (from v in context.Vendors where v.ObjectId == id select v).FirstOrDefault();

            return View();
        }

        /// <summary>
        /// Save edited vendor into db and redirect to vendor index
        /// </summary>
        /// <param name="id">GUID of edited vendor</param>
        /// <param name="collection">FormCollection of provided parameters</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(Guid id, Vendor vendor)
        {
            //try
            //{
            //    DataContext context = new DataContext();
            //    Vendor vendor = (from v in context.Vendors where v.ObjectId == id select v).FirstOrDefault();

            //    if (!ModelState.IsValid)
            //    {
                    
            //        vendor.OrganisationName = collection["OrganisationName"];
            //        context.SaveChanges();
            //        return RedirectToAction("Index");
            //    }
            //    else
            //    {
            //        return View(vendor);
            //    }
            //}
            //catch
            //{
            //    return RedirectToAction("Edit", id);
            //}

            return View();
        }
    }
}
