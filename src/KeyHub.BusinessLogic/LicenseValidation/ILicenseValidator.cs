using System;
using System.Collections.Generic;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    public interface ILicenseValidator
    {
        /// <summary>
        /// Vaidate DomainLicenses
        /// </summary>
        /// <param name="appKey">Customer App key</param>
        /// <param name="domainValidations">List of domains to validate</param>
        /// <returns>List of DomainValidationResult</returns>
        IEnumerable<DomainValidationResult> Validate(Guid appKey, IEnumerable<DomainValidation> domainValidations);
    }
}