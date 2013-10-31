using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;

namespace KeyHub.Integration.Tests
{
    public class LicenseValidationScenario
    {
        public Guid AppKey;
        public Guid FeatureCode;
        public string PublicKeyXml;

        public LicenseValidationScenario()
        {
            using (var dataContext = new DataContext())
            {
                var country = DatabaseUtil.InsertItem(dataContext, dataContext.Countries, new Country()
                {
                    CountryCode = "foo",
                    CountryName = "country.name",
                    NativeCountryName = "country.nativeCountryName"
                });

                var customer = DatabaseUtil.InsertItem(dataContext, dataContext.Customers, new Customer()
                {
                    Name = "customer.name",
                    Street = "customer.street",
                    PostalCode = "customer.postalCode",
                    City = "customer.city",
                    Region = "customer.region",
                    CountryCode = country.CountryCode
                });

                var customerApp = DatabaseUtil.InsertItem(dataContext, dataContext.CustomerApps, new CustomerApp()
                {
                    ApplicationName = "test CustomerApp"
                });

                var customerAppKey = DatabaseUtil.InsertItem(dataContext, dataContext.CustomerAppKeys, new CustomerAppKey()
                {
                    CustomerAppId = customerApp.CustomerAppId
                });

                AppKey = customerAppKey.AppKey;

                var vendor = DatabaseUtil.InsertItem(dataContext, dataContext.Vendors, new Vendor()
                {
                    Name = "vendor.name",
                    Street = "vendor.street",
                    PostalCode = "vendor.postalcode",
                    City = "vendor.city",
                    Region = "vendor.region",
                    CountryCode = country.CountryCode
                });

                var privateKey = new PrivateKey()
                {
                    DisplayName = "I am a private key",
                    VendorId = vendor.ObjectId
                };

                privateKey.SetKeyBytes();

                privateKey = DatabaseUtil.InsertItem(dataContext, dataContext.PrivateKeys, privateKey);

                PublicKeyXml = privateKey.GetPublicKeyXmlString();

                var sku = DatabaseUtil.InsertItem(dataContext, dataContext.SKUs, new SKU()
                {
                    SkuCode = "super sku",
                    PrivateKeyId = privateKey.PrivateKeyId,
                    VendorId = vendor.ObjectId,
                    AutoDomainDuration = 12
                });

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

                var license = DatabaseUtil.InsertItem(dataContext, dataContext.Licenses, new License()
                {
                    OwnerName = "license.ownerName",
                    LicenseExpires = DateTime.Now.AddDays(100),
                    SkuId = sku.SkuId,
                    PurchasingCustomerId = customer.ObjectId,
                    OwningCustomerId = customer.ObjectId
                });

                DatabaseUtil.InsertItem(dataContext, dataContext.LicenseCustomerApps, new LicenseCustomerApp()
                {
                    CustomerAppId = customerApp.CustomerAppId,
                    LicenseId = license.ObjectId
                });
            }
            
        }
    }
}
