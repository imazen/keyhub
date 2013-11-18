using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Web.ViewModels.Feature;
using KeyHub.Data;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the Feature entity
    /// </summary>
    [Authorize]
    public class FeatureController : ControllerBase
    {
        private readonly IDataContextFactory dataContextFactory;
        public FeatureController(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of features
        /// </summary>
        /// <returns>Feature index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Authorized vendors
                var vendorGuids = (from v in context.Vendors select v).Select(x => x.ObjectId).ToList();
                
                //Eager loading feature
                var featureQuery = (from f in context.Features where vendorGuids.Contains(f.VendorId) orderby f.FeatureCode select f)
                    .Include(x => x.Vendor)
                    .OrderBy(x => x.FeatureName);

                FeatureIndexViewModel viewModel = new FeatureIndexViewModel(featureQuery.ToList());
            
                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single feature
        /// </summary>
        /// <returns>Create feature view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var viewModel = FeatureCreateEditViewModel.ForCreate(context);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created feature into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created <c>FeatureViewModel</c></param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(FeatureCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var feature = new Feature();

                    viewModel.ApplyToEntity(context, feature);

                    context.Features.Add(feature);

                    if (context.SaveChanges(CreateValidationFailed))
                    {
                        Flash.Success(String.Format("Feature {0} was created.", feature.FeatureName));
                        return RedirectToAction("Index");
                    }
                }
            }

            //Viewmodel invalid, recall create
            return Create();
        }

        private void CreateValidationFailed(BusinessRuleValidationException businessRuleValidationException)
        {
            foreach (var error in businessRuleValidationException.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
            {
                ModelState.AddModelError("Feature." + error.PropertyName, error.ErrorMessage);
            }
        }

        /// <summary>
        /// Edit a single feature
        /// </summary>
        /// <param name="key">GUID of feature to edit</param>
        /// <returns>Edit feature view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var viewModel = FeatureCreateEditViewModel.ForEdit(context, key);

                if (viewModel == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited feature into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited FeatureViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(FeatureCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    Model.Feature feature = (from f in context.Features where f.FeatureId == viewModel.Feature.FeatureId select f).SingleOrDefault();

                    if (feature == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    
                    viewModel.ApplyToEntity(context, feature);

                    if (context.SaveChanges(CreateValidationFailed))
                    {
                        Flash.Success("The feature was updated.");
                        return RedirectToAction("Index");
                    }
                }
            }

            return Edit(viewModel.Feature.FeatureId);
        }
    }
}
