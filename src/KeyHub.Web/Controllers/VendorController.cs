using System;
using System.Data.Entity;
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
            using (DataContext context = new DataContext())
            {
                var vendorQuery = (from v in context.Vendors select v).Include(x => x.Country);//.FilterByUser(CurrentUser);
                
                VendorIndexViewModel viewModel = new VendorIndexViewModel(vendorQuery.ToList());
                
                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single vendor
        /// </summary>
        /// <returns>Create vendor view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext())
            {
                var countryQuery = from c in context.Countries select c;
                
                VendorCreateViewModel viewModel = new VendorCreateViewModel(countryQuery.ToList());
                
                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created vendor into db and redirect to vendor index
        /// </summary>
        /// <param name="viewModel">Created VendorViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(VendorCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Vendor vendor = viewModel.ToEntity(null);
                        context.Vendors.Add(vendor);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(viewModel);
                }
            }
            catch
            {
                return View(viewModel);
            }
        }

        /// <summary>
        /// Edit a single vendor
        /// </summary>
        /// <param name="key">GUID of vendor to edit</param>
        /// <returns>Edit vendor view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                var vendorQuery = from v in context.Vendors where v.ObjectId == key select v;
                var countryQuery = from c in context.Countries select c;

                VendorEditViewModel viewModel = new VendorEditViewModel(vendorQuery.FirstOrDefault(), countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited vendor into db and redirect to vendor index
        /// </summary>
        /// <param name="viewModel">Edited VendorViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(VendorEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Vendor vendor = (from v in context.Vendors where v.ObjectId == viewModel.Vendor.ObjectId select v).FirstOrDefault();

                        viewModel.ToEntity(vendor);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(viewModel);
                }
            }
            catch
            {
                return View(viewModel);
            }
        }
    }
}
