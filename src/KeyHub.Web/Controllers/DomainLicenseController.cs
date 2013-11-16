using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Web.ViewModels.DomainLicense;
using KeyHub.Data;
using Microsoft.Ajax.Utilities;
using MvcFlash.Core;

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
                var license =
                    (from x in context.Licenses where x.ObjectId == owningLicense select x)
                    .Include(x => x.Sku)
                    .SingleOrDefault();

                if (license == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                var domainLicense = new Model.DomainLicense()
                {
                    LicenseId = license.ObjectId,
                    License = license,
                    DomainLicenseIssued = license.Sku.CalculateDomainIssueDate(),
                    DomainLicenseExpires = license.Sku.CalculateAutoDomainExpiration()
                };

                if (license.Sku.CanCalculateManualDomainExpiration)
                {
                    domainLicense.DomainLicenseExpires = license.Sku.CalculateManualDomainExpiration();
                }
                var viewModel = new DomainLicenseViewModel(domainLicense);

                viewModel.RedirectUrl = new UrlHelper(ControllerContext.RequestContext).Action("Details", "License", new { key = viewModel.LicenseId });

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created domainLicense into db and redirect to domainLicense index
        /// </summary>
        /// <param name="viewModel">Created DomainLicenseViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(DomainLicenseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var license = context.Licenses.Where(l => l.ObjectId == viewModel.LicenseId)
                        .Include(l => l.Sku)
                        .Include(l => l.Sku.PrivateKey)
                        .SingleOrDefault();

                    if (license == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                    var domainLicense = new Model.DomainLicense();

                    domainLicense.License = license;
                    domainLicense.KeyBytes = license.Sku.PrivateKey.KeyBytes;

                    domainLicense.DomainName = viewModel.DomainName;
                    domainLicense.DomainLicenseIssued = viewModel.DomainLicenseIssued;
                    domainLicense.DomainLicenseExpires = viewModel.DomainLicenseExpires;
                    domainLicense.AutomaticlyCreated = viewModel.AutomaticlyCreated;

                    context.DomainLicenses.Add(domainLicense);

                    if (context.SaveChanges(CreateValidationFailed))
                    {
                        Flash.Success("The domain license was created.");
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
                var domainLicense = (from x in context.DomainLicenses where x.DomainLicenseId == key select x)
                    .Include(x => x.License.Sku)
                    .SingleOrDefault();

                if (domainLicense == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                var viewModel = new DomainLicenseViewModel(domainLicense);

                viewModel.UseLocalReferrerAsRedirectUrl(Request);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited domainLicense into db and redirect to domainLicense index
        /// </summary>
        /// <param name="viewModel">Edited DomainLicenseViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(DomainLicenseViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        var domainLicense = (from x in context.DomainLicenses where x.DomainLicenseId == viewModel.DomainLicenseId select x)
                            .SingleOrDefault();

                        if (domainLicense == null)
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                        domainLicense.DomainName = viewModel.DomainName;
                        domainLicense.DomainLicenseIssued = viewModel.DomainLicenseIssued;
                        domainLicense.DomainLicenseExpires = viewModel.DomainLicenseExpires;
                        domainLicense.AutomaticlyCreated = viewModel.AutomaticlyCreated;

                        context.SaveChanges();
                        Flash.Success("The domain license was updated.");
                    }

                    if (!string.IsNullOrEmpty(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Details", "License", new {key = viewModel.LicenseId});
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

        public ActionResult Remove(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                DomainLicense domainLicense;
                ActionResult actionResult = LoadDomainLicenseForRemoval(context, key, out domainLicense);
                if (actionResult != null) return actionResult;

                return View(domainLicense);
            }
        }

        [HttpPost, ActionName("Remove"), ValidateAntiForgeryToken]
        public ActionResult RemovePost(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                DomainLicense domainLicense;
                ActionResult actionResult = LoadDomainLicenseForRemoval(context, key, out domainLicense);
                if (actionResult != null) return actionResult;

                context.DomainLicenses.Remove(domainLicense);
                context.SaveChanges();
                Flash.Success("The domain license was deleted.");

                return RedirectToAction("Details", "License", new { key = domainLicense.LicenseId }); ;
            }
        }

        private static ActionResult LoadDomainLicenseForRemoval(IDataContextByUser context, Guid key, out DomainLicense domainLicense)
        {
            domainLicense = context.DomainLicenses
                .Include(dl => dl.License)
                .Include(dl => dl.License.Sku)
                .SingleOrDefault(x => x.DomainLicenseId == key);

            if (domainLicense == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (!domainLicense.CanBeManuallyDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return null;
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
