using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Represents license validation result per domain
    /// </summary>
    public class DomainValidationResult
    {
        public KeyHub.Client.DomainLicense DomainLicense { get; set; }
        public byte[] KeyBytes { get; set; }
    }
}
