# License Validation

## License request from ImageResizer.dll to KeyHub (these may occur multiple times in an app lifetime on a multi-domain site).

    <licenseRequest>
    <appId>{guid}</appId>
    <domains>
      <domain name="microsoft.com">
        <feature>{guid}</feature>
        <feature>{guid}</feature>
      </domain>
    </domains>
    </licenseRequest>


## Response from KeyHub

    <licenses>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    <license>base64-encoded-license</license>
    </licenses>

In addition to creating (or retrieving) DomainLicense rows, KeyHub should log an ApplicationIssue if it failed to located a valid license for any domain/feature combination. If the ApplicationIssue has not be emailed to subscribed users in the last 2 days, send a notification.


## Decrypted license contents:
  
    Domain: microsoft.com
    Owner: Owner Name
    Issued: utc date-time
    Expires: utc date-time
    Features: {guid},{guid},{guid},{guid},{guid},{guid}
  
  
## ImageResizer 

ImageResizer will cache all provided licenses to disk, eliminating future HTTP requests until those licenses expire.
Expired licenses are automatically deleted from the cache.
  

## Web.config

    <resizer>
        <licenses>
            <auto appId="{GUID}" />
            <local use="mydomain.com" />
            <license>
                BASE64ENCODED   
            </license>
        <licenses>
    </resizer>


## Example resizer-side code

     /// <summary>
        /// Performs a remote request to the license server to get 
        /// </summary>
        /// <param name="domainFeatures"></param>
        /// <returns></returns>
        private List<byte[]> RequestLicenses(Dictionary<string,List<Guid>> domainFeatures){
            var results = new List<byte[]>();
            string appStr = c.get("licenses.auto.appId", null);
            if (string.IsNullOrEmpty(appStr)) return results;

            Guid appId = new Guid(appStr);

            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("licenseReqeust");
            doc.AppendChild(root);
            var appIdElement = doc.CreateElement("appId");
            appIdElement.AppendChild(doc.CreateTextNode( appId.ToString()));
            root.AppendChild(appIdElement);
            var domains = doc.CreateElement("domains");
            root.AppendChild(domains);
            foreach (string domain in domainFeatures.Keys) {
                var d = doc.CreateElement("domain");
                d.SetAttribute("name", domain);
                foreach (Guid g in domainFeatures[domain]) {
                    var feature = doc.CreateElement("feature");
                    feature.AppendChild(doc.CreateTextNode(g.ToString()));
                    d.AppendChild(feature);
                }
                root.AppendChild(d);
            }
            var request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(LicensingUrl);
            request.ContentType = "application/xml";
            request.Method = "POST";
            byte[] body = UTF8Encoding.UTF8.GetBytes(doc.OuterXml);
            request.ContentLength = body.Length;
            using (var upstream = request.GetRequestStream()){
                upstream.Write(body,0,body.Length);
            }
            using (var response = request.GetResponse()) {
                XmlDocument rdoc = new XmlDocument();
                rdoc.Load(response.GetResponseStream());
                foreach(var l in rdoc.DocumentElement.ChildNodes){
                    var lic = l as XmlElement;
                    if (lic != null && lic.Name == "license"){
                        results.Add(Convert.FromBase64String(lic.InnerText.Trim()));
                    }
                }
            }
            return results;
        }
        
## Example public/private key generation

      /// <summary>
        /// Generates a key 2048-bit keypair and returns the xml fragment containing it
        /// </summary>
        /// <returns></returns>
        internal static string GenerateKeyPairXml() {
            using(var r = new RSACryptoServiceProvider(2048))
               return r.ToXmlString(true);
        }
        /// <summary>
        /// Strips the private information from the given key pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        internal static string StripPrivateKey(string pair) {
            using (var r = new RSACryptoServiceProvider(2048)) {
                r.FromXmlString(pair);
                return r.ToXmlString(false);
            }
        }


## Example license serialization and encryption

            internal string SerializeUnencrypted() {
                return "Domain: " + Domain.Replace('\n', ' ') + "\n" +
                        "OwnerName: " + OwnerName.Replace('\n', ' ') + "\n" +
                        "Issued: " + Issued.ToString() + "\n" +
                        "Expires: " + Expires.ToString() + "\n" +
                        "Features: " + Join(Features) + "\n";
            }

            internal byte[] SerializeAndEncrypt(string xmlKeyPair) {
                using (var r = new RSACryptoServiceProvider(2048)) {
                    r.FromXmlString(xmlKeyPair);
                    return r.Encrypt(UTF8Encoding.UTF8.GetBytes(SerializeUnencrypted()), false);
                }
            }
