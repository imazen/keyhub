using System;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    /// <summary>
    /// Represets info for validation
    /// </summary>
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
