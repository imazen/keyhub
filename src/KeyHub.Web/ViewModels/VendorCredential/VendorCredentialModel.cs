using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using KeyHub.Common.Utils;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.VendorCredential
{
    public class VendorCredentialModel
    {
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public Guid? VendorCredentialId { get; set; }
        public string CredentialName { get; set; }
        public string CredentialValue { get; set; }

        public static VendorCredentialModel ForCreate(IDataContextFactory contextFactory, Guid vendorId)
        {
            VendorCredentialModel result;

            using (var context = contextFactory.CreateByUser())
            {
                var vendor = (from x in context.Vendors where x.ObjectId == vendorId select x).FirstOrDefault();

                result = new VendorCredentialModel()
                {
                    VendorId = vendor.ObjectId,
                    VendorName = vendor.Name,
                };
            }
            return result;
        }

        public static VendorCredentialModel ForEdit(IDataContextFactory dataContextFactory, Guid key)
        {
            VendorCredentialModel result;

            using (var dataContext = dataContextFactory.CreateByUser())
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
}