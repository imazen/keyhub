using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data.BusinessRules;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.Feature;
using KeyHub.Data;

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
                var vendorQuery = (from c in context.Vendors select c);//.FilterByUser(UserEntity);

                FeatureCreateViewModel viewModel = new FeatureCreateViewModel(vendorQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created feature into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created <c>FeatureViewModel</c></param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(FeatureCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var feature = viewModel.ToEntity(null);
                    context.Features.Add(feature);

                    if (context.SaveChanges(CreateValidationFailed))
                        return RedirectToAction("Index");
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
                var featureQuery = from f in context.Features where f.FeatureId == key select f;
                var vendorQuery = (from v in context.Vendors select v);//;.FilterByUser(UserEntity);

                FeatureEditViewModel viewModel = new FeatureEditViewModel(featureQuery.FirstOrDefault(), vendorQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited feature into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited FeatureViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(FeatureEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    Model.Feature feature = (from f in context.Features where f.FeatureId == viewModel.Feature.FeatureId select f).FirstOrDefault();
                    viewModel.ToEntity(feature);

                    if (context.SaveChanges(CreateValidationFailed))
                        return RedirectToAction("Index");
                }
            }
            return Edit(viewModel.Feature.FeatureId);
        }
    }
}
