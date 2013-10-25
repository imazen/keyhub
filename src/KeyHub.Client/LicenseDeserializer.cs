using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KeyHub.Client
{
    public class LicenseDeserializer
    {
        public Dictionary<string, List<DomainLicense>> DeserializeAll(string publicKeyXml, ICollection<KeyValuePair<string,string>> licensesAndSignatures)
        {
            var licenses = new Dictionary<string, List<DomainLicense>>(StringComparer.OrdinalIgnoreCase);

            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.PersistKeyInCsp = false;
                r.FromXmlString(publicKeyXml);

                foreach (var licenseAndSignature in licensesAndSignatures)
                {
                    var licenseBytes = Convert.FromBase64String(licenseAndSignature.Key);
                    var domainLicense = new DomainLicense(Encoding.UTF8.GetString(licenseBytes));

                    if (!r.VerifyData(licenseBytes, new SHA256Managed(),
                            Convert.FromBase64String(licenseAndSignature.Value)))
                    {
                        throw new Exception("Signature failed for license of domain " + domainLicense.Domain);
                    }

                    string domain = DomainUtility.NormalizeDomain(domainLicense.Domain);
                    List<DomainLicense> forDomain;
                    if (!licenses.TryGetValue(domain, out forDomain))
                    {
                        forDomain = new List<DomainLicense>();
                        licenses[domain] = forDomain;
                    }
                    forDomain.Add(domainLicense);
                }
            }

            return licenses;
        }
    }
}