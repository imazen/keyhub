using System;
using System.Collections.Generic;
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
            var featureGuid = Guid.NewGuid();
            var domain = "foobar.com";

            Guid skuId;
            Guid customerAppId;

            using (var dataContext = new DataContext())
            {
                var customerApp = new CustomerApp()
                {
                    ApplicationName = "test CustomerApp"
                };

                dataContext.CustomerApps.Add(customerApp);
                dataContext.SaveChanges();

                var customerAppKey = new CustomerAppKey()
                {
                    CustomerAppId = customerApp.CustomerAppId
                };
                dataContext.CustomerAppKeys.Add(customerAppKey);
                dataContext.SaveChanges();

                appKey = customerAppKey.AppKey;

                var country = new Country()
                {
                    CountryCode = "foo",
                    CountryName = "country.name",
                    NativeCountryName = "country.nativeCountryName"
                };

                dataContext.Countries.Add(country);
                dataContext.SaveChanges();

                var vendor = new Vendor()
                {
                    Name = "vendor.name",
                    Street = "vendor.street",
                    PostalCode = "vendor.postalcode",
                    City = "vendor.city",
                    Region = "vendor.region",
                    CountryCode = country.CountryCode
                };
                dataContext.Vendors.Add(vendor);
                dataContext.SaveChanges();

                var privateKey = new PrivateKey()
                {
                    DisplayName = "I am a private key",
                    KeyBytes = new byte[] {1, 2, 3},
                    VendorId = vendor.ObjectId
                };
                dataContext.PrivateKeys.Add(privateKey);
                dataContext.SaveChanges();

                var sku = new SKU()
                {
                    SkuCode = "super sku",
                    PrivateKeyId = privateKey.PrivateKeyId,
                    VendorId = vendor.ObjectId
                };
                dataContext.SKUs.Add(sku);
                dataContext.SaveChanges();

                var feature = new Feature()
                {
                    FeatureName = "feature.featureName",
                    VendorId = vendor.ObjectId,
                    //FeatureCode = featureGuid
                };
                dataContext.Features.Add(feature);
                dataContext.SaveChanges();

                var skuFeature = new SkuFeature()
                {
                    SkuId = sku.SkuId,
                    FeatureId = feature.FeatureId
                };

                dataContext.SkuFeatures.Add(skuFeature);
                dataContext.SaveChanges();

                skuId = sku.SkuId;
                customerAppId = customerApp.CustomerAppId;
            }

            using (var dataContext = new DataContext())
            {
                var license = new License()
                {
                    OwnerName = "license.ownerName",
                    LicenseExpires = DateTime.Now.AddDays(100),
                    SkuId = skuId
                };

                dataContext.Licenses.Add(license);
                dataContext.SaveChanges();  // EF can't determine the order to do things...

                dataContext.LicenseCustomerApps.Add(new LicenseCustomerApp()
                {
                    CustomerAppId = customerAppId,
                    LicenseId = license.ObjectId
                });
                dataContext.SaveChanges();
            }

            var identity = new Mock<IIdentity>(MockBehavior.Loose);
            var loggingService = new Mock<ILoggingService>(MockBehavior.Loose);
            var applicationIssuesUnitOfWork = new Mock<IApplicationIssueUnitOfWork>(MockBehavior.Loose);

            using (var dataContextFactory = new CheckedDataContextFactory(identity.Object))
            {
                var validator = new LicenseValidator(dataContextFactory, loggingService.Object,
                    applicationIssuesUnitOfWork.Object);

                var result = validator.Validate(appKey, new[] {new DomainValidation(domain, featureGuid)}).Single();

                Assert.Equal(result.DomainName, "foorbar.com");
                Assert.Contains(featureGuid, result.Features);
            }
        }
    }
}
