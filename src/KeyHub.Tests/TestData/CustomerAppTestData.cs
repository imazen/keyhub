using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class CustomerAppTestData
    {
        public static CustomerApp Create(CustomerAppKey customerAppKey, License license)
        {
            CustomerApp customerApp = new CustomerApp
            {
                CustomerAppId = Guid.NewGuid(),
                LicenseCustomerApps = new List<LicenseCustomerApp>(),
                CustomerAppKeys = new Collection<CustomerAppKey>()
            };

            customerAppKey.CustomerApp = customerApp;
            customerAppKey.CustomerAppId = customerApp.CustomerAppId;

            customerApp.CustomerAppKeys.Add(customerAppKey);

            LicenseCustomerApp licenseCustomerApp = new LicenseCustomerApp
            {
                CustomerApp = customerApp,
                CustomerAppId = customerApp.CustomerAppId,
                License = license,
                LicenseId = license.ObjectId
            };
            customerApp.LicenseCustomerApps.Add(licenseCustomerApp);

            return customerApp;
        }
    }
}
