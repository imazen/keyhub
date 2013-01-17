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
        public string DomainName { get; set; }

        public string OwnerName { get; set; }

        public DateTime Issued { get; set; }

        public DateTime? Expires { get; set; }

        public List<Guid> Features { get; set; }

        public DomainValidationResult()
        {
            Features = new List<Guid>();
        }

        /// <summary>
        /// Serializes license validation result
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// Domain: microsoft.com
        /// Owner: Owner Name
        /// Issued: utc date-time
        /// Expires: utc date-time
        /// Features: {guid},{guid},{guid},{guid},{guid},{guid}
        /// </example>
        public string Serialize()
        {
            string expires = Expires.HasValue ? Expires.Value.ToUniversalTime().ToString() : string.Empty;
            return "Domain: " + DomainName.Replace('\n', ' ') + "\n" +
                    "OwnerName: " + OwnerName.Replace('\n', ' ') + "\n" +
                    "Issued: " + Issued.ToUniversalTime().ToString() + "\n" +
                    "Expires: " + expires + "\n" +
                    "Features: " + Join(Features) + "\n";
        }

        private static string Join(IEnumerable<Guid> features)
        {
            return string.Join(",", features.Select(x => x.ToString()).ToArray());
        }
    }
}
