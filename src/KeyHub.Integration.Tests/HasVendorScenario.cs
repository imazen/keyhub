using System;
using System.Text;
using KeyHub.Common.Utils;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;

namespace KeyHub.Integration.Tests
{
    public class HasVendorScenario
    {
        public Guid VendorId;
        public string VendorCredential;
        public string PublicKeyXml;
        public Guid FeatureCode;
        public string SkuCode;
        public Guid SkuId;

        public HasVendorScenario()
        {
            using (var dataContext = new DataContext())
            {
                var country = DatabaseUtil.InsertItem(dataContext, dataContext.Countries, new Country()
                {
                    CountryCode = "vendor cc",
                    CountryName = "vendor country.CountryName",
                    NativeCountryName = "vendor country.NativeCountryName"
                });

                var vendor = DatabaseUtil.InsertItem(dataContext, dataContext.Vendors, new Vendor()
                {
                    Name = "vendor.name",
                    Street = "vendor.street",
                    PostalCode = "vendor.postalcode",
                    City = "vendor.city",
                    Region = "vendor.region",
                    CountryCode = country.CountryCode
                });

                VendorId = vendor.ObjectId;

                VendorCredential = "vendorCredential.value";

                DatabaseUtil.InsertItem(dataContext, dataContext.VendorCredentials,
                    new VendorCredential()
                    {
                        CredentialName = "vendorCredential.name",
                        CredentialValue = SymmetricEncryption.EncryptForDatabase(Encoding.UTF8.GetBytes(VendorCredential)),
                        VendorId = vendor.ObjectId,
                    });

                var privateKey = new PrivateKey()
                {
                    DisplayName = "I am a private key",
                    VendorId = vendor.ObjectId
                };

                privateKey.SetKeyBytes();

                privateKey = DatabaseUtil.InsertItem(dataContext, dataContext.PrivateKeys, privateKey);

                PublicKeyXml = privateKey.GetPublicKeyXmlString();

                SkuCode = "super sku";

                var sku = DatabaseUtil.InsertItem(dataContext, dataContext.SKUs, new SKU()
                {
                    SkuCode = SkuCode,
                    PrivateKeyId = privateKey.PrivateKeyId,
                    VendorId = vendor.ObjectId,
                    AutoDomainDuration = 12
                });

                SkuId = sku.SkuId;

                var feature = DatabaseUtil.InsertItem(dataContext, dataContext.Features, new Feature()
                {
                    FeatureName = "feature.featureName",
                    VendorId = vendor.ObjectId
                });

                FeatureCode = feature.FeatureCode;

                DatabaseUtil.InsertItem(dataContext, dataContext.SkuFeatures, new SkuFeature()
                {
                    SkuId = sku.SkuId,
                    FeatureId = feature.FeatureId
                });
            }
        }
    }
}