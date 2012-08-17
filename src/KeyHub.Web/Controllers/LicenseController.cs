﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.License;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the License entity
    /// </summary>
    public class LicenseController : Controller
    {
        /// <summary>
        /// Get list of Licenses
        /// </summary>
        /// <param name="transactionID">Optionally show by transactionID</param>
        /// <returns>License index list view</returns>
        public ActionResult Index(int transactionID = 0)
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading License
                var licenseQuery = (from x in context.Licenses select x).Include(x => x.PurchasingCustomer)
                    .Include(x => x.OwningCustomer).Include(x => x.Sku);
                
                LicenseIndexViewModel viewModel = new LicenseIndexViewModel(licenseQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get details of Licenses
        /// </summary>
        /// <param name="key">Guid of license to show</param>
        /// <returns>License details view</returns>
        public ActionResult Details(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading License
                var licenseQuery = (from x in context.Licenses where x.ObjectId == key select x).Include(x => x.PurchasingCustomer)
                    .Include(x => x.OwningCustomer).Include(x => x.Sku);

                LicenseDetailsViewModel viewModel = new LicenseDetailsViewModel(licenseQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single License
        /// </summary>
        /// <returns>Create License view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext())
            {
                var skuQuery = from x in context.SKUs select x;
                var customerQuery = from x in context.Customers select x;

                LicenseCreateViewModel viewModel = new LicenseCreateViewModel(skuQuery.ToList(), customerQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created License into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created LicenseCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(LicenseCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.License license = viewModel.ToEntity(null);
                        context.Licenses.Add(license);

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
                throw;
            }
        }

        /// <summary>
        /// Edit a single License
        /// </summary>
        /// <param name="key">Key of the license to edit</param>
        /// <returns>Edit License view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                var licenseQuery = from x in context.Licenses where x.ObjectId == key select x;
                var skuQuery = from x in context.SKUs select x;
                var customerQuery = from x in context.Customers select x;

                LicenseEditViewModel viewModel = new LicenseEditViewModel(licenseQuery.FirstOrDefault(),
                    skuQuery.ToList(), customerQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited License into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited LicenseEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(LicenseEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.License license = (from x in context.Licenses where x.ObjectId == viewModel.License.ObjectId select x).FirstOrDefault();
                        viewModel.ToEntity(license);

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
                throw;
            }
        }
    }
}