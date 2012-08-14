using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.PrivateKey;

namespace KeyHub.Web.Controllers
{
    public class PrivateKeyController : Controller
    {
        /// <summary>
        /// Get list of privatekeys
        /// </summary>
        /// <returns>PrivateKey index partial list view</returns>
        /// <param name="parentVendor">Guid of the vendor to show private keys for</param>
        public ActionResult IndexPartial(Guid parentVendor)
        {
            using (DataContext context = new DataContext())
            {
                var privateKeyQuery = (from x in context.PrivateKeys where x.VendorId == parentVendor orderby x.DisplayName select x);
                var vendorQuery = (from v in context.Vendors where v.ObjectId == parentVendor select v);

                PrivateKeyIndexViewModel viewModel = new PrivateKeyIndexViewModel(privateKeyQuery.ToList(), vendorQuery.FirstOrDefault());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single privateKey
        /// </summary>
        /// <param name="parentVendor">GUID of assiciated vendor</param>
        /// <returns>Create privateKey view</returns>
        public ActionResult Create(Guid parentVendor)
        {
            using (DataContext context = new DataContext())
            {
                var vendorQuery = from x in context.Vendors where x.ObjectId == parentVendor select x;

                PrivateKeyCreateViewModel viewModel = new PrivateKeyCreateViewModel(vendorQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created privateKey into db and redirect to privateKey index
        /// </summary>
        /// <param name="viewModel">Created PrivateKeyViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(PrivateKeyCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.PrivateKey privateKey = viewModel.ToEntity(null);
                        context.PrivateKeys.Add(privateKey);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Details", "Vendor", new { key = viewModel.PrivateKey.VendorId });
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
        /// Edit a single privateKey
        /// </summary>
        /// <param name="key">GUID of privateKey to edit</param>
        /// <returns>Edit privateKey view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                var privateKeyQuery = from x in context.PrivateKeys where x.PrivateKeyId == key select x;

                PrivateKeyEditViewModel viewModel = new PrivateKeyEditViewModel(privateKeyQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited privateKey into db and redirect to privateKey index
        /// </summary>
        /// <param name="viewModel">Edited PrivateKeyViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(PrivateKeyEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.PrivateKey privateKey = (from x in context.PrivateKeys where x.PrivateKeyId == viewModel.PrivateKey.PrivateKeyId select x).FirstOrDefault();

                        viewModel.ToEntity(privateKey);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Details", "Vendor", new { key = viewModel.PrivateKey.VendorId });
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
