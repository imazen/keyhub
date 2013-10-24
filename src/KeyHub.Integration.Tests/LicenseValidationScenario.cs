﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Model;

namespace KeyHub.Integration.Tests
{
    public class LicenseValidationScenario
    {
        public Guid AppKey;
        public Guid FeatureCode;

        public LicenseValidationScenario()
        {
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
                    Region = "customer.region",
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

                AppKey = customerAppKey.AppKey;

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
                    KeyBytes = new byte[] { 1, 2, 3 },
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

                FeatureCode = feature.FeatureCode;

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
            
        }

        private static TItem InsertItem<TItem>(DataContext dataContext, IDbSet<TItem> dbSet, TItem item) where TItem : class
        {
            dbSet.Add(item);
            dataContext.SaveChanges();
            return item;
        }
    }
}
