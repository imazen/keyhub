using KeyHub.Model;
using System.Collections.Generic;

namespace KeyHub.BusinessLogic.LicenseValidation.Validators
{
    /// <summary>
    /// Specifies doamin license validation
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validate domain licenses
        /// </summary>
        /// <param name="domainLicenses"></param>
        /// <returns></returns>
        List<DomainLicense> Validate(IEnumerable<DomainLicense> domainLicenses);
    }
}
