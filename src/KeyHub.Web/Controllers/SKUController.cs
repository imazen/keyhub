using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data.BusinessRules;
using KeyHub.Web.ViewModels.SKU;
using KeyHub.Data;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the SKU entity
    /// </summary>
    [Authorize]
    public class SKUController : ControllerBase
    {
        private readonly IDataContextFactory dataContextFactory;
        public SKUController(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of SKUs
        /// </summary>
        /// <returns>SKU index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
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
            using (var context = dataContextFactory.CreateByUser())
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
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(SKUCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var sku = viewModel.ToEntity(null);
                    context.SKUs.Add(sku);

                    //Offload adding SKUFeatures to Dynamic SKU Model
                    sku.AddFeatures(viewModel.GetNewFeatureGUIDs());

                    if (context.SaveChanges(CreateValidationFailed))
                    {
                        Flash.Success(string.Format("Sku {0} was created.", sku.SkuCode));
                        return RedirectToAction("Index");
                    }
                }
                
            }
            return Create();
        }

        private void CreateValidationFailed(BusinessRuleValidationException businessRuleValidationException)
        {
            foreach (var error in businessRuleValidationException.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
            {
                ModelState.AddModelError("SKU." + error.PropertyName, error.ErrorMessage);
            }
        }

        /// <summary>
        /// Edit a single SKU
        /// </summary>
        /// <param name="key">GUID of SKU to edit</param>
        /// <returns>Edit SKU view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
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
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(SKUEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    Model.SKU sku =
                        (from x in context.SKUs where x.SkuId == viewModel.SKU.SkuId select x).FirstOrDefault();

                    viewModel.ToEntity(sku);

                    //Offload adding and removing SKUFeatures to Dynamic SKU Model
                    sku.AddFeatures(viewModel.GetNewFeatureGUIDs(sku));
                    sku.RemoveFeatures(viewModel.GetRemovedFeatureGUIDs(sku));

                    if (context.SaveChanges(CreateValidationFailed))
                        return RedirectToAction("Index");
                }
            }
            return Edit(viewModel.SKU.SkuId);
        }
    }
}
