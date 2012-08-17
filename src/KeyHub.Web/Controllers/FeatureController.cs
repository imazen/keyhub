using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.Feature;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the Feature entity
    /// </summary>
    public class FeatureController : Controller
    {
        /// <summary>
        /// Get list of features
        /// </summary>
        /// <returns>Feature index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading feature
                var featureQuery = (from f in context.Features orderby f.FeatureCode select f).Include(x => x.Vendor);

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
            using (DataContext context = new DataContext())
            {
                var vendorQuery = from c in context.Vendors select c;

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
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.Feature feature = viewModel.ToEntity(null);
                        context.Features.Add(feature);
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
        /// Edit a single feature
        /// </summary>
        /// <param name="key">GUID of feature to edit</param>
        /// <returns>Edit feature view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext())
            {
                var featureQuery = from f in context.Features where f.FeatureId == key select f;
                var vendorQuery = from v in context.Vendors select v;

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
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        Model.Feature feature = (from f in context.Features where f.FeatureId == viewModel.Feature.FeatureId select f).FirstOrDefault();

                        viewModel.ToEntity(feature);

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
