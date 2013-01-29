using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Add processing logic to CustomerApp
    /// </summary>
    public partial class CustomerApp : IModelItem
    {
        /// <summary>
        /// Add LicenseCustomerApp to CustomerApp based on new License Guids
        /// </summary>
        /// <param name="NewLicenseGuids">List of new license Guids to add</param>
        public void AddLicenses(IEnumerable<Guid> NewLicenseGuids)
        {
            foreach (Guid newLicenseGuid in NewLicenseGuids)
            {
                if (!(from x in this.LicenseCustomerApps where x.LicenseId == newLicenseGuid && x.CustomerAppId == this.CustomerAppId select x).Any())
                {
                    this.LicenseCustomerApps.Add(new LicenseCustomerApp()
                                                     {LicenseId = newLicenseGuid, CustomerAppId = this.CustomerAppId});
                }
            }
        }

        /// <summary>
        /// Remove LicenseCustomerApp from CustomerApp based on removed feature Guids
        /// </summary>
        /// <param name="RemovedLicenseGuids">List of license Guids to remove</param>
        public void RemoveLicenses(IEnumerable<Guid> RemovedLicenseGuids)
        {
            foreach (Guid removedLicenseGuid in RemovedLicenseGuids)
            {
                LicenseCustomerApp removedLicense = (from f in this.LicenseCustomerApps where f.LicenseId == removedLicenseGuid select f).FirstOrDefault();
                this.LicenseCustomerApps.Remove(removedLicense);
            }
        }
    }
}
