using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KeyHub.Client
{
    public class DomainLicense
    {
        internal DomainLicense(string licenseText)
        {
            string[] lines = licenseText.Split('\n');
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

        public string Domain { get; private set; }
        public string OwnerName { get; private set; }
        public DateTime Issued { get; private set; }
        public DateTime Expires { get; private set; }
        public IList<Guid> Features { get; private set; }

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
    }
}