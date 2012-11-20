﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.SKU;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the SKU entity
    /// </summary>
    [Authorize]
    public class SKUController : ControllerBase
    {
        /// <summary>
        /// Get list of SKUs
        /// </summary>
        /// <returns>SKU index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading SKU
                var SKUQuery = (from x in context.SKUs select x).Include(x => x.PrivateKey)
                               .Include(x => x.SkuFeatures.Select(f => f.Feature))
                               .OrderBy(x => x.SkuCode);

                SKUIndexViewModel viewModel = new SKUIndexViewModel(SKUQuery.ToList());
                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single SKU
        /// </summary>
        /// <returns>Create SKU view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var vendorGuids = (from v in context.Vendors select v)
                    .Select(x => x.ObjectId).ToList();

                var vendorQuery = (from x in context.Vendors select x);
                var privateKeyQuery = from x in context.PrivateKeys where vendorGuids.Contains(x.VendorId) orderby x.DisplayName select x;
                var featureQuery = from x in context.Features where vendorGuids.Contains(x.VendorId) orderby x.FeatureName select x;

                var viewModel = new SKUCreateViewModel(vendorQuery.ToList(), privateKeyQuery.ToList(), 
                    featureQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created SKU into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created SKUViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(SKUCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new DataContext(User.Identity))
                {
                    var sku = viewModel.ToEntity(null);
                    context.SKUs.Add(sku);

                    //Offload adding SKUFeatures to Dynamic SKU Model
                    sku.AddFeatures(viewModel.GetNewFeatureGUIDs());

                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return Create();
        }

        /// <summary>
        /// Edit a single SKU
        /// </summary>
        /// <param name="key">GUID of SKU to edit</param>
        /// <returns>Edit SKU view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = new DataContext(User.Identity))
            {
                var vendorGuids = (from v in context.Vendors select v)//.FilterByUser(UserEntity)
                    .Select(x => x.ObjectId).ToList();

                var skuQuery = from x in context.SKUs where x.SkuId == key select x;
                var vendorQuery = (from x in context.Vendors select x);//.FilterByUser(UserEntity);
                var privateKeyQuery = from x in context.PrivateKeys where vendorGuids.Contains(x.VendorId) select x;
                var featureQuery = from x in context.Features where vendorGuids.Contains(x.VendorId) select x;

                var viewModel = new SKUEditViewModel(skuQuery.FirstOrDefault(), vendorQuery.ToList(),
                    privateKeyQuery.ToList(), featureQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited SKU into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited SKUViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(SKUEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        Model.SKU sku = (from x in context.SKUs where x.SkuId == viewModel.SKU.SkuId select x).FirstOrDefault();

                        viewModel.ToEntity(sku);

                        //Offload adding and removing SKUFeatures to Dynamic SKU Model
                        sku.AddFeatures(viewModel.GetNewFeatureGUIDs(sku));
                        sku.RemoveFeatures(viewModel.GetRemovedFeatureGUIDs(sku));

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
