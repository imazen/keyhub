using System;
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
    public class SKUController : Controller
    {
        /// <summary>
        /// Get list of SKUs
        /// </summary>
        /// <returns>SKU index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading SKU
                var SKUQuery = (from x in context.SKUs select x).Include(x => x.PrivateKey)
                                    .Include(x => x.SkuFeatures.Select(f => f.Feature));

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
            using (DataContext context = new DataContext())
            {
                var vendorQuery = from x in context.Vendors select x;
                var privateKeyQuery = from x in context.PrivateKeys orderby x.DisplayName select x;
                var featureQuery = from x in context.Features orderby x.FeatureCode select x;

                SKUCreateViewModel viewModel = new SKUCreateViewModel(vendorQuery.ToList(), privateKeyQuery.ToList(), 
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
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.SKU sku = viewModel.ToEntity(null);
                        context.SKUs.Add(sku);

                        //Offload adding SKUFeatures to Dynamic SKU Model
                        sku.AddFeatures(viewModel.GetNewFeatureGUIDs());

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
        /// Edit a single SKU
        /// </summary>
        /// <param name="key">GUID of SKU to edit</param>
        /// <returns>Edit SKU view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                var skuQuery = from x in context.SKUs where x.SkuId == key select x;
                var vendorQuery = from x in context.Vendors select x;
                var privateKeyQuery = from x in context.PrivateKeys select x;
                var featureQuery = from x in context.Features select x;

                SKUEditViewModel viewModel = new SKUEditViewModel(skuQuery.FirstOrDefault(), vendorQuery.ToList(),
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
                    using (DataContext context = new DataContext())
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
