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

                var vendorScenario = new HasVendorScenario();
                FeatureCode = vendorScenario.FeatureCode;
                PublicKeyXml = vendorScenario.PublicKeyXml;

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

                var license = DatabaseUtil.InsertItem(dataContext, dataContext.Licenses, new License()
                {
                    OwnerName = "license.ownerName",
                    LicenseExpires = DateTime.Now.AddDays(100),
                    SkuId = vendorScenario.SkuId,
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
