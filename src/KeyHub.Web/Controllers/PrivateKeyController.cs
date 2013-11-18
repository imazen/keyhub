using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;
using KeyHub.Web.ViewModels.PrivateKey;
using KeyHub.Data;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the PrivateKey entity
    /// </summary>
    [Authorize]
    public class PrivateKeyController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public PrivateKeyController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of privatekeys
        /// </summary>
        /// <returns>PrivateKey index partial list view</returns>
        /// <param name="parentVendor">Guid of the vendor to show private keys for</param>
        [ChildActionOnly]
        public ActionResult IndexPartial(Guid parentVendor)
        {
            using (var context = dataContextFactory.Create())
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
            using (var context = dataContextFactory.CreateByUser())
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
        [ValidateInput(false), ValidateAntiForgeryToken]
        public ActionResult Create(PrivateKeyCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        var privateKey = new Model.PrivateKey();
                        privateKey.SetKeyBytes();

                        privateKey.DisplayName = viewModel.DisplayName;
                        privateKey.Vendor = context.Vendors.Single(v => v.ObjectId == viewModel.VendorId);
                        context.PrivateKeys.Add(privateKey);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Details", "Vendor", new { key = viewModel.VendorId });
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
            using (var context = dataContextFactory.CreateByUser())
            {
                var privateKey = context.PrivateKeys.SingleOrDefault(k => k.PrivateKeyId == key);

                if (privateKey == null)
                    return new HttpNotFoundResult();

                PrivateKeyEditViewModel viewModel = new PrivateKeyEditViewModel(privateKey);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited privateKey into db and redirect to privateKey index
        /// </summary>
        /// <param name="viewModel">Edited PrivateKeyViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        [ValidateInput(false), ValidateAntiForgeryToken]
        public ActionResult Edit(PrivateKeyEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.Create())
                    {
                        var privateKey = context.PrivateKeys.SingleOrDefault(k => k.PrivateKeyId == viewModel.PrivateKey.PrivateKeyId);

                        if (privateKey == null)
                            return new HttpNotFoundResult();

                        privateKey.DisplayName = viewModel.PrivateKey.DisplayName;

                        context.SaveChanges();
                        Flash.Success("Private key was updated.");
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
        /// Remove a single privateKey
        /// </summary>
        /// <param name="key">GUID of privateKey to remove</param>
        /// <returns></returns>
        public ActionResult Remove(Guid key)
        {
            using (var context = dataContextFactory.Create())
            {
                context.PrivateKeys.Remove(x => x.PrivateKeyId == key);
                context.SaveChanges();
 
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}
