using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;


namespace KeyHub.Client
{
    public class LicenseDownloader
    {
        /// <summary>
        /// Performs a remote request to the license server to get 
        /// </summary>
        /// <param name="domainFeatures"></param>
        /// <returns></returns>
        public List<byte[]> RequestLicenses(Uri licensingUrl, Guid appId, Dictionary<string, List<Guid>> domainFeatures)
        {
            var results = new List<byte[]>();

            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("licenseRequest");
            doc.AppendChild(root);
            var appIdElement = doc.CreateElement("appId");
            appIdElement.AppendChild(doc.CreateTextNode(appId.ToString()));
            root.AppendChild(appIdElement);
            var domains = doc.CreateElement("domains");
            root.AppendChild(domains);
            foreach (string domain in domainFeatures.Keys)
            {
                var d = doc.CreateElement("domain");
                d.SetAttribute("name", domain);
                foreach (Guid g in domainFeatures[domain])
                {
                    var feature = doc.CreateElement("feature");
                    feature.AppendChild(doc.CreateTextNode(g.ToString()));
                    d.AppendChild(feature);
                }
                domains.AppendChild(d);
            }
            var request = (System.Net.HttpWebRequest)WebRequest.Create((Uri) licensingUrl);
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";
            byte[] body = UTF8Encoding.UTF8.GetBytes(doc.OuterXml);
            request.ContentLength = body.Length;
            using (var upstream = request.GetRequestStream())
            {
                upstream.Write(body, 0, body.Length);
            }
            using (var response = request.GetResponse())
            {
                XmlDocument rdoc = new XmlDocument();
                rdoc.Load(response.GetResponseStream());
                foreach (var l in rdoc.DocumentElement.ChildNodes)
                {
                    var lic = l as XmlElement;
                    if (lic != null && lic.Name == "license")
                    {
                        results.Add(Convert.FromBase64String(lic.InnerText.Trim()));
                    }
                }
            }
            return results;
        }
    }
}