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
            public Guid VendorId;
            public string VendorName;
            public Guid? VendorSecretId;
            public string CredentialName;
            public string CredentialValue;
        }

        public ActionResult Create(Guid parentVendor)
        {
            using (var context = dataContextFactory.Create())
            {
                var vendor = (from x in context.Vendors where x.ObjectId == parentVendor select x).FirstOrDefault();

                return View(new VendorSecretEditModel()
                {
                    VendorId = vendor.ObjectId,
                    VendorName = vendor.Name,
                });
            }
        }

        [HttpPost]
        public ActionResult Create(VendorSecretEditModel model)
        {
            throw new NotImplementedException();
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