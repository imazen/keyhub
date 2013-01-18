using System;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Represets info for validation
    /// </summary>
    public class DomainValidation
    {
        public DomainValidation(string domain, Guid features)
        {
            DomainName = domain;
            FeatureCode = features;
        }

        public string DomainName { get; set; }
        public Guid FeatureCode { get; set; }
    }  
}
