using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Web.ViewModels.CustomerApp;
using KeyHub.Data;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the CustomerApp entity
    /// </summary>
    [Authorize]
    public class CustomerAppController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public CustomerAppController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of CustomerApps
        /// </summary>
        /// <returns>CustomerApp index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading CustomerApp, includes Licenses and from License the SKUs
                var customerAppQuery = (from x in context.CustomerApps select x).Include(x => x.LicenseCustomerApps)
                                       .Include(x => x.LicenseCustomerApps.Select(s => s.License))
                                       .Include(x => x.CustomerAppIssues)
                                       .OrderBy(x => x.ApplicationName);

                var viewModel = new CustomerAppIndexViewModel(customerAppQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get partial list of customer apps by transactionID
        /// </summary>
        /// <param name="transactionId">TransactionID to show customer apps for for</param>
        /// <returns>CustomerApp index list view</returns>
        [ChildActionOnly]
        public ActionResult IndexPartial(Guid transactionId)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading License
                var licensesByTransaction = (from l in context.Licenses
                                             where
                                                 l.TransactionItems.FirstOrDefault().TransactionId == transactionId
                                             select l.ObjectId).ToList();

                var customerAppByLicense = (from c in context.LicenseCustomerApps
                                               where
                                                 licensesByTransaction.Contains(c.LicenseId)
                                               select c.CustomerAppId).ToList();
                
                var customerApps = (from x in context.CustomerApps
                                        where customerAppByLicense.Contains(x.CustomerAppId)
                                        select x)
                                        .Include(x => x.LicenseCustomerApps)
                                        .Include(x => x.LicenseCustomerApps.Select(s => s.License))
                                        .Include(x => x.CustomerAppKeys)
                                        .ToList();

                var viewModel = new CustomerAppIndexViewModel(customerApps);

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single CustomerApp
        /// </summary>
        /// <returns>Create CustomerApp view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                return View(CustomerAppCreateViewModel.ForCreate(context));
            }
        }

        /// <summary>
        /// Save created CustomerApp into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created CustomerAppCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CustomerAppCreateViewModel viewModel)
        {
            using (var context = this.dataContextFactory.CreateByUser())
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.TryToSaveCustomerApp(context, (key, message) => ModelState.AddModelError(key, message)))
                    {
                        Flash.Success("The licensed application was created.");
                        return RedirectToAction("Index");
                    }
                }

                var model = CustomerAppCreateViewModel.ForCreate(context);
                model.ApplicationName = viewModel.ApplicationName;
                model.SelectedLicenseGUIDs = viewModel.SelectedLicenseGUIDs;

                return View(model);
            }
        }

        /// <summary>
        /// Edit a single CustomerApp
        /// </summary>
        /// <param name="key">Key of the CustomerApp to edit</param>
        /// <returns>Edit CustomerApp view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var model = CustomerAppCreateViewModel.ForEdit(context, key);

                if (model == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                return View("Create", model);
            }
        }

        /// <summary>
        /// Save edited CustomerApp into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited CustomerAppEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerAppCreateViewModel viewModel)
        {
            using (var context = this.dataContextFactory.CreateByUser())
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.TryToSaveCustomerApp(context, (key, message) => ModelState.AddModelError(key, message)))
                    {
                        Flash.Success("The licensed application was updated.");
                        return RedirectToAction("Index");
                    }
                }

                var model = CustomerAppCreateViewModel.ForEdit(context, viewModel.ApplicationId.Value);
                model.ApplicationName = viewModel.ApplicationName;
                model.SelectedLicenseGUIDs = viewModel.SelectedLicenseGUIDs;

                return View("Create", model);
            }
        }

        /// <summary>
        /// Remove a single CustomerApp
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Remove(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                context.LicenseCustomerApps.Remove(x => x.CustomerAppId == key);
                context.CustomerAppKeys.Remove(x => x.CustomerAppId == key);
                context.CustomerApps.Remove(x => x.CustomerAppId == key);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// Get details of Licenses
        /// </summary>
        /// <param name="key">Guid of customerapp to show</param>
        /// <returns>CustomerApp details view</returns>
        public ActionResult Details(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading License
                var appQuery = (from x in context.CustomerApps where x.CustomerAppId == key select x);

                var viewModel = new CustomerAppViewModel(appQuery.FirstOrDefault());

                return View(viewModel);
            }
        }
    }
}
