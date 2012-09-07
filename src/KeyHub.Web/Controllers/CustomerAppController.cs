using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.CustomerApp;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the CustomerApp entity
    /// </summary>
    [Authorize]
    public class CustomerAppController : Controller
    {
        /// <summary>
        /// Get list of CustomerApps
        /// </summary>
        /// <returns>CustomerApp index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading CustomerApp, includes Licenses and from License the SKUs
                var customerAppQuery = (from x in context.CustomerApps select x).Include(x => x.LicenseCustomerApps)
                                       .Include(x => x.LicenseCustomerApps.Select(s => s.License))
                                       .OrderBy(x => x.ApplicationName);

                CustomerAppIndexViewModel viewModel = new CustomerAppIndexViewModel(customerAppQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get partial list of customer apps by transactionID
        /// </summary>
        /// <param name="transactionId">TransactionID to show customer apps for for</param>
        /// <returns>CustomerApp index list view</returns>
        public ActionResult IndexPartial(int transactionId)
        {
            using (var context = new DataContext(User.Identity))
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
                
                var customerAppQuery = (from x in context.CustomerApps
                                        where customerAppByLicense.Contains(x.CustomerAppId)
                                        select x)
                                        .Include(x => x.LicenseCustomerApps)
                                        .Include(x => x.LicenseCustomerApps.Select(s => s.License));

                var viewModel = new CustomerAppIndexViewModel(customerAppQuery.ToList());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single CustomerApp
        /// </summary>
        /// <returns>Create CustomerApp view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //License will be recognized by SKU, so eager load SKU
                var licenseQuery = (from x in context.Licenses select x).Include(x => x.Sku);

                CustomerAppCreateViewModel viewModel = new CustomerAppCreateViewModel(licenseQuery.ToList());

                viewModel.RedirectUrl = Request.UrlReferrer.ToString();

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created CustomerApp into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created CustomerAppCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(CustomerAppCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        Model.CustomerApp customerApp = viewModel.ToEntity(null);
                        context.CustomerApps.Add(customerApp);

                        //Offload adding CustomerAppLicenses to Dynamic SKU Model
                        customerApp.AddLicenses(viewModel.GetNewLicenseGUIDs());

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
        /// Edit a single CustomerApp
        /// </summary>
        /// <param name="key">Key of the CustomerApp to edit</param>
        /// <returns>Edit CustomerApp view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var customerAppQuery = from x in context.CustomerApps where x.CustomerAppId == key select x;
                var licenseQuery = from x in context.Licenses select x;

                CustomerAppEditViewModel viewModel = new CustomerAppEditViewModel(customerAppQuery.FirstOrDefault(),
                    licenseQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited CustomerApp into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited CustomerAppEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(CustomerAppEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        Model.CustomerApp customerApp = (from x in context.CustomerApps where x.CustomerAppId == viewModel.CustomerApp.CustomerAppId select x).FirstOrDefault();
                        viewModel.ToEntity(customerApp);

                        //Offload adding and removing LicenseCustomerApps to Dynamic CustomerApp Model
                        customerApp.AddLicenses(viewModel.GetNewLicenseGUIDs(customerApp));
                        customerApp.RemoveLicenses(viewModel.GetRemovedLicenseGUIDs(customerApp));

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
