using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.BusinessLogic.BusinessRules;
using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Core.Logging;
using KeyHub.Data;
using KeyHub.Data.ApplicationIssues;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class LicenseValidatorTests
    {
        [Fact]
        [CleanDatabase]
        public void CanValidateLicense()
        {
            Guid appKey;
            Guid featureCode;
            var domain = "foobar.com";

            using (var dataContext = new DataContext())
            {
                var country = InsertItem(dataContext, dataContext.Countries, new Country()
                {
                    CountryCode = "foo",
                    CountryName = "country.name",
                    NativeCountryName = "country.nativeCountryName"
                });

                var customer = InsertItem(dataContext, dataContext.Customers, new Customer()
                {
                    Name = "customer.name",
                    Street = "customer.street",
                    PostalCode = "customer.postalCode",
                    City = "customer.city",
                    Region =  "customer.region",
                    CountryCode = country.CountryCode
                });
 
                var customerApp = InsertItem(dataContext, dataContext.CustomerApps, new CustomerApp()
                {
                    ApplicationName = "test CustomerApp"
                });

                var customerAppKey = InsertItem(dataContext, dataContext.CustomerAppKeys, new CustomerAppKey()
                {
                    CustomerAppId = customerApp.CustomerAppId
                });

                appKey = customerAppKey.AppKey;

                var vendor = InsertItem(dataContext, dataContext.Vendors, new Vendor()
                {
                    Name = "vendor.name",
                    Street = "vendor.street",
                    PostalCode = "vendor.postalcode",
                    City = "vendor.city",
                    Region = "vendor.region",
                    CountryCode = country.CountryCode
                });

                var privateKey = InsertItem(dataContext, dataContext.PrivateKeys, new PrivateKey()
                {
                    DisplayName = "I am a private key",
                    KeyBytes = new byte[] {1, 2, 3},
                    VendorId = vendor.ObjectId
                });

                var sku = InsertItem(dataContext, dataContext.SKUs, new SKU()
                {
                    SkuCode = "super sku",
                    PrivateKeyId = privateKey.PrivateKeyId,
                    VendorId = vendor.ObjectId
                });

                var feature = InsertItem(dataContext, dataContext.Features, new Feature()
                {
                    FeatureName = "feature.featureName",
                    VendorId = vendor.ObjectId
                });

                featureCode = feature.FeatureCode;

                InsertItem(dataContext, dataContext.SkuFeatures, new SkuFeature()
                {
                    SkuId = sku.SkuId,
                    FeatureId = feature.FeatureId
                });

                var license = InsertItem(dataContext, dataContext.Licenses, new License()
                {
                    OwnerName = "license.ownerName",
                    LicenseExpires = DateTime.Now.AddDays(100),
                    SkuId = sku.SkuId,
                    PurchasingCustomerId = customer.ObjectId,
                    OwningCustomerId = customer.ObjectId
                });

                InsertItem(dataContext, dataContext.LicenseCustomerApps, new LicenseCustomerApp()
                {
                    CustomerAppId = customerApp.CustomerAppId,
                    LicenseId = license.ObjectId
                });
            }

            var identity = new Mock<IIdentity>(MockBehavior.Loose);
            var loggingService = new Mock<ILoggingService>(MockBehavior.Loose);
            var applicationIssuesUnitOfWork = new Mock<IApplicationIssueUnitOfWork>(MockBehavior.Loose);

            using (var dataContextFactory = new CheckedDataContextFactory(identity.Object))
            {
                var validator = new LicenseValidator(dataContextFactory, loggingService.Object,
                    applicationIssuesUnitOfWork.Object);

                var result = validator.Validate(appKey, new[] {new DomainValidation(domain, featureCode)}).Single();

                Assert.Equal(result.DomainName, domain);
                Assert.Contains(featureCode, result.Features);
            }
        }

        private static TItem InsertItem<TItem>(DataContext dataContext, IDbSet<TItem> dbSet, TItem country1) where TItem : class
        {
            var country = country1;

            dbSet.Add(country);
            dataContext.SaveChanges();
            return country;
        }
    }
}
