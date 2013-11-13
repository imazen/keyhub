using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Web.ViewModels.DomainLicense;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for DomainLicense
    /// </summary>
    [Authorize]
    public class DomainLicenseController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public DomainLicenseController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of domain licenses
        /// </summary>
        /// <returns>DomainLicense index partial list view</returns>
        /// <param name="parentLicense">Guid of the license to show domains for</param>
        [ChildActionOnly]
        public ActionResult IndexPartial(Guid parentLicense)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var domainLicenseQuery = (from x in context.DomainLicenses where x.LicenseId == parentLicense orderby x.DomainName select x)
                    .Include(x => x.License.Sku);

                var viewModel = new DomainLicenseIndexViewModel(parentLicense, domainLicenseQuery.ToList());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single domainLicense
        /// </summary>
        /// <param name="owningLicense">Owning license ID</param>
        /// <returns>Create DomainLicense view</returns>
        public ActionResult Create(Guid owningLicense)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var licenseQuery =
                    (from x in context.Licenses where x.ObjectId == owningLicense select x)
                    .Include(x => x.Sku);

                var viewModel = new DomainLicenseCreateViewModel(licenseQuery.FirstOrDefault());

                viewModel.RedirectUrl = new UrlHelper(ControllerContext.RequestContext).Action("Details", "License", new { key = viewModel.DomainLicense.LicenseId });

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created domainLicense into db and redirect to domainLicense index
        /// </summary>
        /// <param name="viewModel">Created DomainLicenseViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(DomainLicenseCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var license = context.Licenses.Where(l => l.ObjectId == viewModel.DomainLicense.LicenseId)
                        .Include(l => l.Sku)
                        .Include(l => l.Sku.PrivateKey)
                        .SingleOrDefault();

                    DomainLicense domainLicense = viewModel.ToEntity(null);
                    domainLicense.KeyBytes = license.Sku.PrivateKey.KeyBytes;

                    context.DomainLicenses.Add(domainLicense);

                    if (context.SaveChanges(CreateValidationFailed))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                }
            }

            return View(viewModel);
        }

        /// <summary>
        /// Edit a single domainLicense
        /// </summary>
        /// <param name="key">GUID of domainLicense to edit</param>
        /// <returns>Edit domainLicense view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var domainLicenseQuery = (from x in context.DomainLicenses where x.DomainLicenseId == key select x)
                    .Include(x => x.License.Sku);

                var viewModel = new DomainLicenseEditViewModel(domainLicenseQuery.FirstOrDefault());

                viewModel.RedirectUrl = Request.UrlReferrer.ToString();

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited domainLicense into db and redirect to domainLicense index
        /// </summary>
        /// <param name="viewModel">Edited DomainLicenseViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(DomainLicenseEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        var domainLicense = (from x in context.DomainLicenses where x.DomainLicenseId == viewModel.DomainLicense.DomainLicenseId select x).FirstOrDefault();

                        viewModel.ToEntity(domainLicense);

                        context.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Details", "License", new {key = viewModel.DomainLicense.LicenseId});
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
        /// Remove a single domainLicense
        /// </summary>
        /// <param name="key">GUID of domainLicense to edit</param>
        /// <returns></returns>
        public ActionResult Remove(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                DomainLicense domainLicense = context.DomainLicenses.FirstOrDefault(x => x.DomainLicenseId == key);
                context.DomainLicenses.Remove(domainLicense);
                context.SaveChanges();

                return RedirectToAction("Details", "License", new { key = domainLicense.LicenseId }); ;
            }
        }

        private void CreateValidationFailed(BusinessRuleValidationException businessRuleValidationException)
        {
            foreach (var error in businessRuleValidationException.ValidationResults.Where(x => x != BusinessRuleValidationResult.Success))
            {
                ModelState.AddModelError(error.PropertyName != null ? "DomainLicense." + error.PropertyName : string.Empty, error.ErrorMessage);
            }
        }

    }
}
