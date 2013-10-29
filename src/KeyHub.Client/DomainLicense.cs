using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KeyHub.Client
{
    public class DomainLicense
    {
        public DomainLicense(
            string domain, 
            string ownerName, 
            DateTime issued, 
            DateTime? expires,
            IList<Guid> features)
        {
            Domain = domain;
            OwnerName = ownerName;
            Issued = issued;
            Expires = expires;
            Features = features;
        }

        internal DomainLicense()
        {
        }
             
        public string Domain { get; private set; }
        public string OwnerName { get; private set; }
        public DateTime Issued { get; private set; }
        public DateTime? Expires { get; private set; }
        public IList<Guid> Features { get; private set; }

        public static DomainLicense Parse(string licenseText)
        {
            var result = new DomainLicense();

            string[] lines = licenseText.Split('\n');
            foreach (string l in lines)
            {
                int colon = l.IndexOf(':');
                if (colon < 1) continue;
                string key = l.Substring(0, colon).Trim().ToLowerInvariant();
                string value = l.Substring(colon + 1).Trim();

                switch (key)
                {
                    case "domain": result.Domain = value; break;
                    case "owner": result.OwnerName = value; break;
                    case "issued": result.Issued = DateTime.Parse(value); break;
                    case "expires":

                        if (value.Trim().Length == 0)
                        {
                            result.Expires = null;
                        }
                        else
                        {
                            result.Expires = DateTime.Parse(value);
                        }

                        break;
                    case "features":
                        List<Guid> ids = new List<Guid>();
                        string[] parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string p in parts)
                        {
                            ids.Add(new Guid(p));
                        }
                        result.Features = ids;
                        break;
                }
            }

            return result;
        }

        public string SerializeUnencrypted()
        {
            string expires = Expires.HasValue ? Expires.Value.ToUniversalTime().ToString() : string.Empty;

            return "Domain: " + Domain.Replace('\n', ' ') + "\n" +
                   "OwnerName: " + OwnerName.Replace('\n', ' ') + "\n" +
                   "Issued: " + Issued.ToString() + "\n" +
                   "Expires: " + expires + "\n" +
                   "Features: " + Join(Features) + "\n";
        }

        internal string Join(ICollection<Guid> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Guid g in items)
                sb.Append(g.ToString() + ",");
            return sb.ToString().TrimEnd(',');
        }
    }
}