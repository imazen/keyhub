using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using KeyHub.Common.Utils;
using KeyHub.Data;
using KeyHub.Model;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    public class VendorCredentialController : Controller
    {
        private IDataContextFactory dataContextFactory;

        public VendorCredentialController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public class VendorCredentialModel
        {
            public Guid VendorId { get; set; }
            public string VendorName { get; set; }
            public Guid? VendorCredentialId { get; set; }
            public string CredentialName { get; set; }
            public string CredentialValue { get; set; }

            public static VendorCredentialModel ForVendor(Guid parentVendor, IDataContextFactory contextFactory)
            {
                VendorCredentialModel result;

                using (var context = contextFactory.Create())
                {
                    var vendor = (from x in context.Vendors where x.ObjectId == parentVendor select x).FirstOrDefault();

                    result = new VendorCredentialModel()
                    {
                        VendorId = vendor.ObjectId,
                        VendorName = vendor.Name,
                    };
                }
                return result;
            }

            public static VendorCredentialModel ForVendorCredential(IDataContextFactory dataContextFactory, Guid key)
            {
                VendorCredentialModel result;

                using (var dataContext = dataContextFactory.Create())
                {
                    var vendorCredential =
                        dataContext.VendorCredentials.Where(vs => vs.VendorCredentialId == key).Include(x => x.Vendor).Single();

                    result = new VendorCredentialModel()
                    {
                        VendorId = vendorCredential.Vendor.ObjectId,
                        VendorName = vendorCredential.Vendor.Name,
                        VendorCredentialId = vendorCredential.VendorCredentialId,
                        CredentialName = vendorCredential.CredentialName,
                        CredentialValue = Encoding.UTF8.GetString(SymmetricEncryption.DecryptForDatabase(vendorCredential.CredentialValue))
                    };
                }
                return result;
            }
        }

        public ActionResult Create(Guid parentVendor)
        {
            var model = VendorCredentialModel.ForVendor(parentVendor, dataContextFactory);

            return View("CreateEdit", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorCredentialModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForVendor(inputModel.VendorId, dataContextFactory);
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
            var model = VendorCredentialModel.ForVendorCredential(dataContextFactory, key);

            return View("CreateEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorCredentialModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForVendorCredential(dataContextFactory, inputModel.VendorCredentialId.Value);
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
            var model = VendorCredentialModel.ForVendorCredential(dataContextFactory, key);

            return View("Remove", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid VendorId, Guid VendorCredentialId)
        {
            if (!ModelState.IsValid)
            {
                var resultModel = VendorCredentialModel.ForVendorCredential(dataContextFactory, VendorCredentialId);
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