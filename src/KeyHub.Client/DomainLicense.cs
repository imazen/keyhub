using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KeyHub.Client
{
    public class DomainLicense
    {
        internal DomainLicense(string publicKeyXml,byte[] encryptedLicense, LicenseDecrypter v)
        {
            Decrypted = v.Decrypt(publicKeyXml, encryptedLicense);
            Encrypted = encryptedLicense;
            string[] lines = Decrypted.Split('\n');
            foreach (string l in lines)
            {
                int colon = l.IndexOf(':');
                if (colon < 1) continue;
                string key = l.Substring(0, colon).Trim().ToLowerInvariant();
                string value = l.Substring(colon + 1).Trim();

                switch (key)
                {
                    case "domain": Domain = value; break;
                    case "owner": OwnerName = value; break;
                    case "issued": Issued = DateTime.Parse(value); break;
                    case "expires": Expires = DateTime.Parse(value); break;
                    case "features":
                        List<Guid> ids = new List<Guid>();
                        string[] parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string p in parts)
                        {
                            ids.Add(new Guid(p));
                        }
                        Features = ids;
                        break;
                }
            }
        }

        internal string Decrypted { get; private set; }
        internal byte[] Encrypted { get; private set; }
        internal string Domain { get; private set; }
        internal string OwnerName { get; private set; }
        internal DateTime Issued { get; private set; }
        internal DateTime Expires { get; private set; }
        internal IList<Guid> Features { get; private set; }

        internal string Join(ICollection<Guid> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Guid g in items)
                sb.Append(g.ToString() + ",");
            return sb.ToString().TrimEnd(',');
        }

        internal string SerializeUnencrypted()
        {
            return "Domain: " + Domain.Replace('\n', ' ') + "\n" +
                   "OwnerName: " + OwnerName.Replace('\n', ' ') + "\n" +
                   "Issued: " + Issued.ToString() + "\n" +
                   "Expires: " + Expires.ToString() + "\n" +
                   "Features: " + Join(Features) + "\n";
        }

        internal byte[] SerializeAndEncrypt(string xmlKeyPair)
        {
            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.FromXmlString(xmlKeyPair);
                return r.Encrypt(Encoding.UTF8.GetBytes(SerializeUnencrypted()), false);
            }
        }
        /// <summary>
        /// Returns a human readable, single-line description of the license
        /// </summary>
        /// <returns></returns>
        internal string GetShortDescription()
        {
            StringBuilder sb = new StringBuilder(OwnerName + " - " + Domain + " - " + Issued.ToString() + " - " + Expires.ToString() + " - ");
            foreach (var id in Features)
                sb.Append(LicenseVerifier.GetFriendlyName(id) + " ");
            return sb.ToString().TrimEnd();
        }
    }
}