using System;
using System.Linq;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Model;

namespace KeyHub.Web.Controllers
{
    public class VendorSecretController : Controller
    {
        private IDataContextFactory dataContextFactory;

        public VendorSecretController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public class VendorSecretEditModel
        {
            public Guid VendorId { get; set; }
            public string VendorName { get; set; }
            public Guid? VendorSecretId { get; set; }
            public string CredentialName { get; set; }
            public string CredentialValue { get; set; }

            public static VendorSecretEditModel ForVendor(Guid parentVendor, IDataContextFactory contextFactory)
            {
                VendorSecretEditModel vendorSecretEditModel;

                using (var context = contextFactory.Create())
                {
                    var vendor = (from x in context.Vendors where x.ObjectId == parentVendor select x).FirstOrDefault();

                    vendorSecretEditModel = new VendorSecretEditModel()
                    {
                        VendorId = vendor.ObjectId,
                        VendorName = vendor.Name,
                    };
                }
                return vendorSecretEditModel;
            }

        }

        public ActionResult Create(Guid parentVendor)
        {
            var vendorSecretEditModel = VendorSecretEditModel.ForVendor(parentVendor, dataContextFactory);

            return View(vendorSecretEditModel);
        }


        [HttpPost]
        public ActionResult Create(VendorSecretEditModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var vendorSecretEditModel = VendorSecretEditModel.ForVendor(viewModel.VendorId, dataContextFactory);
                vendorSecretEditModel.CredentialName = viewModel.CredentialName;
                vendorSecretEditModel.CredentialValue = viewModel.CredentialValue;
                return View(vendorSecretEditModel);
            }

            using (var dataContext = dataContextFactory.Create())
            {
                dataContext.VendorSecrets.Add(new VendorSecret()
                {
                    VendorId = viewModel.VendorId,
                    Name = viewModel.CredentialName,
                    SharedSecret = viewModel.CredentialValue
                });
                dataContext.SaveChanges();
            }

            return RedirectToAction("Details", "Vendor", new {key = viewModel.VendorId});
        }

        public ActionResult Edit(Guid key)
        {
            throw new NotImplementedException();
        }

        public ActionResult Remove(Guid key)
        {
            throw new NotImplementedException();
        }
    }
}