using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace KeyHub.Client
{
    public class LicenseDecrypter
    {
        private static object decryptionLock = new object();
        private static RSACryptoServiceProvider crypto;

        public Dictionary<string, List<DomainLicense>> DecryptAll(string publicKeyXml, ICollection<byte[]> encrypted)
        {
            var licenses = new Dictionary<string, List<DomainLicense>>(StringComparer.OrdinalIgnoreCase);

            foreach (byte[] data in encrypted)
            {
                var d = new DomainLicense(publicKeyXml, data, this);
                string domain = DomainUtility.NormalizeDomain(d.Domain);
                List<DomainLicense> forDomain;
                if (!licenses.TryGetValue(domain, out forDomain))
                {
                    forDomain = new List<DomainLicense>();
                    licenses[domain] = forDomain;
                }
                forDomain.Add(d);
            }
            return licenses;
        }

        internal string Decrypt(string publicKeyXml, byte[] data)
        {
            lock (decryptionLock)
            {
                if (crypto == null)
                {
                    crypto = new RSACryptoServiceProvider(2048);
                    crypto.FromXmlString(publicKeyXml);
                }

                byte[] decrypted = crypto.Decrypt(data, false);
                return System.Text.UTF8Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}