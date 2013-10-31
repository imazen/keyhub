using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class CanLoadVendorWithSecrets
    {
        [Fact]
        [CleanDatabase]
        public void CanSaveVendorWithSecrets()
        {
            var expectedName = "firstKey!";
            var expectedSecret = "foobarbaz12334";
            Guid vendorId;

            using (var dataContext = new DataContext())
            {
                var country = DatabaseUtil.InsertItem(dataContext, dataContext.Countries, new Country()
                {
                    CountryCode = "foo",
                    CountryName = "country.name",
                    NativeCountryName = "country.nativeCountryName"
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

                vendorId = vendor.ObjectId;

                DatabaseUtil.InsertItem(dataContext, dataContext.VendorSecrets, new VendorSecret()
                {
                    VendorId = vendor.ObjectId,
                    Name = expectedName,
                    SharedSecret = expectedSecret
                });
            }

            using (var dataContext = new DataContext())
            {
                var vendor = dataContext.Vendors.Where(v => v.ObjectId == vendorId)
                    .Include(x => x.VendorSecrets).Single();

                var vendorSecret = vendor.VendorSecrets.Single();

                Assert.Equal(expectedName, vendorSecret.Name);
                Assert.Equal(expectedSecret, vendorSecret.SharedSecret);
            }
        } 
    }
}
