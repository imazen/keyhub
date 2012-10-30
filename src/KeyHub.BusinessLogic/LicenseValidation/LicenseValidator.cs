using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Validates licenses
    /// </summary>
    public static class LicenseValidator
    {
        /// <summary>
        /// Validate licenses
        /// </summary>
        /// <param name="customerAppId">AppId to validate</param>
        /// <param name="domains"></param>
        /// <returns></returns>
        public static string ValidateLicense(Guid customerAppId, IEnumerable<DomainValidation> domains)
        {
            using (var context = new DataContext())
            {
                //var application = (from x in context.CustomerApps where x.CustomerAppId == customerAppId select x).Include(x => x.LicenseCustomerApps.li).FirstOrDefault();

                return "";
            }
        }
    }

    public class DomainValidation
    {
        public DomainValidation(string domain, Guid[] features)
        {
            this.DomainName = domain;
            this.FeatureCodes = features;
        }

        public string DomainName { get; set; }
        public Guid[] FeatureCodes { get; set; }
    }  
}
