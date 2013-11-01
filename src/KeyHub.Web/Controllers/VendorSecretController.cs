using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Model;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    public class VendorSecretController : Controller
    {
        private IDataContextFactory dataContextFactory;

        public VendorSecretController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public class VendorSecretModel
        {
            public Guid VendorId { get; set; }
            public string VendorName { get; set; }
            public Guid? VendorSecretId { get; set; }
            public string CredentialName { get; set; }
            public string CredentialValue { get; set; }

            public static VendorSecretModel ForVendor(Guid parentVendor, IDataContextFactory contextFactory)
            {
                VendorSecretModel vendorSecretEditModel;

                using (var context = contextFactory.Create())
                {
                    var vendor = (from x in context.Vendors where x.ObjectId == parentVendor select x).FirstOrDefault();

                    vendorSecretEditModel = new VendorSecretModel()
                    {
                        VendorId = vendor.ObjectId,
                        VendorName = vendor.Name,
                    };
                }
                return vendorSecretEditModel;
            }

            public static VendorSecretModel ForVendorSecret(IDataContextFactory dataContextFactory, Guid key)
            {
                VendorSecretModel vendorSecretEditModel;

                using (var dataContext = dataContextFactory.Create())
                {
                    var vendorSecret =
                        dataContext.VendorSecrets.Where(vs => vs.VendorSecretId == key).Include(x => x.Vendor).Single();

                    vendorSecretEditModel = new VendorSecretModel()
                    {
                        VendorId = vendorSecret.Vendor.ObjectId,
                        VendorName = vendorSecret.Vendor.Name,
                        VendorSecretId = vendorSecret.VendorSecretId,
                        CredentialName = vendorSecret.Name,
                        CredentialValue = vendorSecret.SharedSecret
                    };
                }
                return vendorSecretEditModel;
            }
        }

        public ActionResult Create(Guid parentVendor)
        {
            var model = VendorSecretModel.ForVendor(parentVendor, dataContextFactory);

            return View("CreateEdit", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorSecretModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorSecretModel.ForVendor(inputModel.VendorId, dataContextFactory);
                resultModel.CredentialName = inputModel.CredentialName;
                resultModel.CredentialValue = inputModel.CredentialValue;
                return View("CreateEdit", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                dataContext.VendorSecrets.Add(new VendorSecret()
                {
                    VendorId = inputModel.VendorId,
                    Name = inputModel.CredentialName,
                    SharedSecret = inputModel.CredentialValue
                });
                dataContext.SaveChanges();
            }

            Flash.Success("VendorSecret was successfully saved.");
            return RedirectToAction("Details", "Vendor", new { key = inputModel.VendorId });
        }

        public ActionResult Edit(Guid key)
        {
            var vendorSecretEditModel = VendorSecretModel.ForVendorSecret(dataContextFactory, key);

            return View("CreateEdit", vendorSecretEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorSecretModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorSecretModel.ForVendorSecret(dataContextFactory, inputModel.VendorSecretId.Value);
                resultModel.CredentialName = inputModel.CredentialName;
                resultModel.CredentialValue = inputModel.CredentialValue;
                return View("CreateEdit", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                var vendorSecret =
                    dataContext.VendorSecrets.Where(x => x.VendorSecretId == inputModel.VendorSecretId.Value).Single();

                vendorSecret.Name = inputModel.CredentialName;
                vendorSecret.SharedSecret = inputModel.CredentialValue;

                dataContext.SaveChanges();
            }

            Flash.Success("VendorSecret was successfully modified.");
            return RedirectToAction("Details", "Vendor", new { key = inputModel.VendorId });
        }

        [HttpGet]
        public ActionResult Remove(Guid key)
        {
            var model = VendorSecretModel.ForVendorSecret(dataContextFactory, key);

            return View("Remove", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid VendorId, Guid VendorSecretId)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorSecretModel.ForVendorSecret(dataContextFactory, VendorSecretId);
                return View("Remove", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                dataContext.VendorSecrets.Remove(s => s.VendorSecretId == VendorSecretId);
                dataContext.SaveChanges();
            }

            Flash.Success("VendorSecret was successfully deleted.");
            return RedirectToAction("Details", "Vendor", new { key = VendorId });
        }
    }
}