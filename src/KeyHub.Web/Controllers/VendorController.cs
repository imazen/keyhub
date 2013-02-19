using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.Vendor;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for Vendor entity
    /// </summary>
    [Authorize]
    public class VendorController : ControllerBase
    {
        private readonly IDataContextFactory dataContextFactory;
        public VendorController(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of vendors
        /// </summary>
        /// <returns>Vendor index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Vendor
                var vendorQuery = (from v in context.Vendors select v).Include(x => x.Country);//.FilterByUser(UserEntity);
                
                var viewModel = new VendorIndexViewModel(context.GetUser(HttpContext.User.Identity), vendorQuery.ToList());
                
                return View(viewModel);
            }
        }

        /// <summary>
        /// Get details of one specific vendor
        /// </summary>
        /// <param name="key">GUID of vender to view</param>
        /// <returns>Vendor details view</returns>
        public ActionResult Details(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Vendor
                var vendorQuery = (from v in context.Vendors where v.ObjectId == key select v).Include(x => x.Country);

                VendorDetailsViewModel viewModel = new VendorDetailsViewModel(vendorQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single vendor
        /// </summary>
        /// <returns>Create vendor view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
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
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        Vendor vendor = viewModel.ToEntity(null);
                        context.Vendors.Add(vendor);

                        context.SaveChanges();
                        return RedirectToAction("Details", new { key = vendor.ObjectId });
                    }
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
        /// Edit a single vendor
        /// </summary>
        /// <param name="key">GUID of vendor to edit</param>
        /// <returns>Edit vendor view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
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
                    using (var context = dataContextFactory.CreateByUser())
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
                throw;
            }
        }
    }
}
