using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.DomainLicense;
using KeyHub.Web.ViewModels.License;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the License entity
    /// </summary>
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public LicenseController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of Licenses
        /// </summary>
        /// <returns>License index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading License
                var licenseQuery = (from x in context.Licenses select x).Include(x => x.PurchasingCustomer)
                    .Include(x => x.OwningCustomer)
                    .Include(x => x.Sku)
                    .OrderBy(x => x.OwningCustomer.Name);
                
                LicenseIndexViewModel viewModel = new LicenseIndexViewModel(licenseQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get list of Licenses
        /// </summary>
        /// <param name="transactionId">TransactionID to show licenses for</param>
        /// <returns>License index list view</returns>
        public ActionResult IndexPartial(int transactionId)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading License
                var licenseQuery = (from l in context.Licenses
                                    where l.TransactionItems.FirstOrDefault().TransactionId == transactionId
                                    select l)
                                    .Include(x => x.PurchasingCustomer)
                                    .Include(x => x.OwningCustomer)
                                    .Include(x => x.Sku)
                                    .Include(x => x.Domains);

                var viewModel = new LicenseIndexViewModel(licenseQuery.ToList());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Get details of Licenses
        /// </summary>
        /// <param name="key">Guid of license to show</param>
        /// <returns>License details view</returns>
        public ActionResult Details(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading License
                var licenseQuery = (from x in context.Licenses where x.ObjectId == key select x).Include(x => x.PurchasingCustomer)
                    .Include(x => x.OwningCustomer).Include(x => x.Sku);

                LicenseDetailsViewModel viewModel = new LicenseDetailsViewModel(licenseQuery.FirstOrDefault());

                viewModel.RedirectUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty;

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single License
        /// </summary>
        /// <returns>Create License view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var skuQuery = from x in context.SKUs select x;
                var customerQuery = from x in context.Customers select x;

                LicenseCreateViewModel viewModel = new LicenseCreateViewModel(skuQuery.ToList(), customerQuery.ToList());

                viewModel.RedirectUrl = Request.UrlReferrer.ToString();

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created License into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created LicenseCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(LicenseCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        Model.License license = viewModel.ToEntity(null);
                        context.Licenses.Add(license);

                        context.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
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
        /// Edit a single License
        /// </summary>
        /// <param name="key">Key of the license to edit</param>
        /// <returns>Edit License view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var licenseQuery = from x in context.Licenses where x.ObjectId == key select x;
                var skuQuery = from x in context.SKUs select x;
                var customerQuery = from x in context.Customers select x;

                LicenseEditViewModel viewModel = new LicenseEditViewModel(licenseQuery.FirstOrDefault(),
                    skuQuery.ToList(), customerQuery.ToList());

                viewModel.RedirectUrl = Request.UrlReferrer.ToString();

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited License into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited LicenseEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(LicenseEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = dataContextFactory.CreateByUser())
                    {
                        Model.License license = (from x in context.Licenses where x.ObjectId == viewModel.License.ObjectId select x).FirstOrDefault();
                        viewModel.ToEntity(license);

                        context.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
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
    }
}
