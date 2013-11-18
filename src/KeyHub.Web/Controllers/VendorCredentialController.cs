using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using KeyHub.Common.Utils;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Web.ViewModels.VendorCredential;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    [Authorize]
    public class VendorCredentialController : Controller
    {
        private IDataContextFactory dataContextFactory;

        public VendorCredentialController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public ActionResult Create(Guid parentVendor)
        {
            var model = VendorCredentialModel.ForCreate(dataContextFactory, parentVendor);

            return View("CreateEdit", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorCredentialModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForCreate(dataContextFactory, inputModel.VendorId);
                resultModel.CredentialName = inputModel.CredentialName;
                resultModel.CredentialValue = inputModel.CredentialValue;
                return View("CreateEdit", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                dataContext.VendorCredentials.Add(new VendorCredential()
                {
                    VendorId = inputModel.VendorId,
                    CredentialName = inputModel.CredentialName,
                    CredentialValue = SymmetricEncryption.EncryptForDatabase(Encoding.UTF8.GetBytes(inputModel.CredentialValue))
                });
                dataContext.SaveChanges();
            }

            Flash.Success("VendorCredential was successfully saved.");
            return RedirectToAction("Details", "Vendor", new { key = inputModel.VendorId });
        }

        public ActionResult Edit(Guid key)
        {
            var model = VendorCredentialModel.ForEdit(dataContextFactory, key);

            return View("CreateEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorCredentialModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForEdit(dataContextFactory, inputModel.VendorCredentialId.Value);
                resultModel.CredentialName = inputModel.CredentialName;
                resultModel.CredentialValue = inputModel.CredentialValue;
                return View("CreateEdit", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                var vendorCredential =
                    dataContext.VendorCredentials.Where(x => x.VendorCredentialId == inputModel.VendorCredentialId.Value).Single();

                vendorCredential.CredentialName = inputModel.CredentialName;
                vendorCredential.CredentialValue =
                    SymmetricEncryption.EncryptForDatabase(Encoding.UTF8.GetBytes(inputModel.CredentialValue));

                dataContext.SaveChanges();
            }

            Flash.Success("VendorCredential was successfully modified.");
            return RedirectToAction("Details", "Vendor", new { key = inputModel.VendorId });
        }

        [HttpGet]
        public ActionResult Remove(Guid key)
        {
            var model = VendorCredentialModel.ForEdit(dataContextFactory, key);

            return View("Remove", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid VendorId, Guid VendorCredentialId)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForEdit(dataContextFactory, VendorCredentialId);
                return View("Remove", resultModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                dataContext.VendorCredentials.Remove(s => s.VendorCredentialId == VendorCredentialId);
                dataContext.SaveChanges();
            }

            Flash.Success("VendorCredential was successfully deleted.");
            return RedirectToAction("Details", "Vendor", new { key = VendorId });
        }
    }
}