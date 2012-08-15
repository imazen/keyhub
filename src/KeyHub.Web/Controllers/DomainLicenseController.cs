using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.DomainLicense;

namespace KeyHub.Web.Controllers
{
    public class DomainLicenseController : Controller
    {
        /// <summary>
        /// Get list of domain licenses
        /// </summary>
        /// <returns>DomainLicense index partial list view</returns>
        /// <param name="parentLicense">Guid of the license to show domains for</param>
        public ActionResult IndexPartial(Guid parentLicense)
        {
            using (DataContext context = new DataContext())
            {
                var domainLicenseQuery = (from x in context.DomainLicenses where x.LicenseId == parentLicense orderby x.DomainName select x);
                var licenseQuery = (from v in context.Licenses where v.ObjectId == parentLicense select v);

                DomainLicenseIndexViewModel viewModel = new DomainLicenseIndexViewModel(domainLicenseQuery.ToList());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single domainLicense
        /// </summary>
        /// <param name="key">GUID of assiciated license</param>
        /// <returns>Create DomainLicense view</returns>
        public ActionResult Create(Guid key)
        {
            throw new NotImplementedException();
            //using (DataContext context = new DataContext())
            //{
            //    var licenseQuery = from x in context.Licenses where x.ObjectId == key select x;

            //    DomainLicenseCreateViewModel viewModel = new DomainLicenseCreateViewModel(licenseQuery.FirstOrDefault());

            //    return View(viewModel);
            //}
        }

        ///// <summary>
        ///// Save created domainLicense into db and redirect to domainLicense index
        ///// </summary>
        ///// <param name="viewModel">Created DomainLicenseViewModel</param>
        ///// <returns>Redirectaction to index if successfull</returns>
        //[HttpPost]
        //public ActionResult Create(DomainLicenseCreateViewModel viewModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            using (DataContext context = new DataContext())
        //            {
        //                Model.DomainLicense domainLicense = viewModel.ToEntity(null);
        //                context.DomainLicenses.Add(domainLicense);

        //                context.SaveChanges();
        //            }
        //            return RedirectToAction("Details", "License", new { key = viewModel.DomainLicense.LicenseId });
        //        }
        //        else
        //        {
        //            return View(viewModel);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Edit a single domainLicense
        ///// </summary>
        ///// <param name="key">GUID of domainLicense to edit</param>
        ///// <returns>Edit domainLicense view</returns>
        //public ActionResult Edit(Guid key)
        //{
        //    using (DataContext context = new DataContext())
        //    {
        //        var domainLicenseQuery = from x in context.DomainLicenses where x.DomainLicenseId == key select x;

        //        DomainLicenseEditViewModel viewModel = new DomainLicenseEditViewModel(domainLicenseQuery.FirstOrDefault());

        //        return View(viewModel);
        //    }
        //}

        ///// <summary>
        ///// Save edited domainLicense into db and redirect to domainLicense index
        ///// </summary>
        ///// <param name="viewModel">Edited DomainLicenseViewModel</param>
        ///// <returns>Redirectaction to index if successfull</returns>
        //[HttpPost]
        //public ActionResult Edit(DomainLicenseEditViewModel viewModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            using (DataContext context = new DataContext())
        //            {
        //                Model.DomainLicense domainLicense = (from x in context.DomainLicenses where x.DomainLicenseId == viewModel.DomainLicense.DomainLicenseId select x).FirstOrDefault();

        //                viewModel.ToEntity(domainLicense);

        //                context.SaveChanges();
        //            }
        //            return RedirectToAction("Details", "License", new { key = viewModel.DomainLicense.LicenseId });
        //        }
        //        else
        //        {
        //            return View(viewModel);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
