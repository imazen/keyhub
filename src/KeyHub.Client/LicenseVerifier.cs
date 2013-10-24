﻿using System;
using System.Collections.Generic;
using System.Text;
using ImageResizer;
using ImageResizer.Plugins;
using ImageResizer.Configuration;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using ImageResizer.Configuration.Xml;
using System.Web.Hosting;
using System.Xml;
using ImageResizer.Configuration.Issues;
using System.IO;

namespace KeyHub.Client
{

    /// <summary>
    /// Contains the methods called by plugins which have enforced licensing.
    /// </summary>
    internal class Methods
    {

        private static LicenseVerifier CreateVerifier()
        {
            return new LicenseVerifier()
            {
                PublicKeyXml =
                    "<RSAKeyValue><Modulus>5ZDHZe0r2oVwEUx5OnKjjT99RWmebgws1HhOOI6YiMRJ4QiIDgmMZy9O7I9RfdxcZ0xkFhMXSGzF4wJVIeNtnWwOm2/cu/wZntyB8wSrPinOArTjQoGNIHsVzCjtd+XwPIqEm/e0dy" +
                    "bdK4UAFm3NskPGFUNmHrx2P8va/9vWHtnhlASy5PncGZKZBlnTuZ8DSBFS7ZwIstShvRri92hTbpo/f0oMqCVGJjo1kZGHSoUFXtGsKtKg9ubVDyXsLutgvzo8XgeS9s9LdOxR0WZNYZkO2E45wwKf10W3EgDlex" +
                    "+gxjfAuVnCHnVwJ4UEIhf5HG2qU6OcuCX/5ZYBDaDhjQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                LicensingUrl = new Uri("http://keyhub.lucrasoft-staging.nl/GetLicenses"),
                VerificationInterval = new TimeSpan(0, 0, 5)
            };
        }


        private static Dictionary<Config, Dictionary<Guid, long>> tracker;
        private static object lockTracker = new object();
        /// <summary>
        /// Notifies the licensing service that the given feature is active in the given app configuration, 
        /// so that it may take appropriate verification steps asynchronously, or schedule the request for watermarking upon failure.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="feature"></param>
        /// <param name="featureDisplayName"></param>
        internal static void NotifyUse(Config config, Guid feature)
        {
            //We only deal with ASP.NET requests. Outside of an HTTP request, no licensing is enforced
            if (HttpContext.Current == null) return;

            //Map local requests if configured
            string domain = HttpContext.Current.Request.Url.DnsSafeHost;
            if (HttpContext.Current.Request.IsLocal)
            {
                domain = config.get("licenses.local.use", null);
                //We can't continue if we don't have a domain to work with
                if (domain == null) return;
            }
            //Locate master license service
            ILicenseService s = config.Plugins.GetOrInstall<ILicenseService>(CreateVerifier());

            //Verify the ILicenseService instance hasn't been replaced or tampered with since startup
            lock (lockTracker)
            {
                long seed = feature.ToString().GetHashCode();
                long auth = seed;
                if (tracker == null) tracker = new Dictionary<Config, Dictionary<Guid, long>>();
                Dictionary<Guid, long> ct;
                if (!tracker.TryGetValue(config, out ct))
                {
                    ct = new Dictionary<Guid, long>();
                    tracker.Add(config, ct);
                }
                else if (!ct.TryGetValue(feature, out auth)) auth = seed;
                long response = s.VerifyAuthenticity(feature);
                if (response != auth) throw new ImageProcessingException("Licensing service responded incorrectly; tampering suspected.");
                tracker[config][feature] = auth + 1;
            }
            //Inform the license service that the specified feature is in use for the given domain and configuration
            s.NotifyUse(domain, feature);
        }

        internal static void SetFeatureName(Config config, Guid id, string displayName)
        {
            ILicenseService s = config.Plugins.GetOrInstall<ILicenseService>(CreateVerifier());
            s.SetFriendlyName(id, displayName);
        }
    }

    /// <summary>
    /// Embeddable implementation of ILicenseService
    /// </summary>
    internal class LicenseVerifier : ILicenseService
    {
        Config config;

        internal LicenseVerifier()
        {
            VerificationInterval = new TimeSpan(0, 10, 0);//10 mins
        }

        internal Uri LicensingUrl { get; set; }
        internal string PublicKeyXml { get; set; }
        internal TimeSpan VerificationInterval { get; set; }

        public string GetLicensingOverview(bool forceVerification)
        {
            if (forceVerification) this.Verify(null);
            StringBuilder sb = new StringBuilder();
            var rejected = GetFeatureDomainPairsWithState(FeatureState.Rejected);
            foreach (var p in rejected)
                sb.AppendLine("Failed to license feature " + GetFriendlyName(p.Value) + " for domain " + p.Key);

            ///TODO: list valid licenses 
            return sb.ToString();
        }


        /// <summary>
        /// The state of the domain/feature combination
        /// </summary>
        private enum FeatureState
        {
            /// <summary>
            /// A valid license is on-file for this feature/domain combo
            /// </summary>
            Enabled,
            /// <summary>
            /// This feature/domain combo has not yet been checked for licensing. 
            /// </summary>
            Pending,
            /// <summary>
            /// No valid licenses could be found for this feature/domain
            /// </summary>
            Rejected
        }

        private string _rejectionKey = null;
        /// <summary>
        /// The Context.Items[] key set for requests that failed licensing
        /// </summary>
        private string RejectionKey
        {
            get
            {
                if (_rejectionKey == null)
                {
                    string n = "reject-" + new Random().Next(234352);
                    //Double check after Random() to minimize possibility of race condition. 
                    //Worst case scenario - one more image not watermarked, not worth lock() overhead.
                    if (_rejectionKey == null) _rejectionKey = n;
                }
                return _rejectionKey;
            }
        }

        //Feature display names API
        private static Dictionary<Guid, string> featureNames = new Dictionary<Guid, string>();
        private static object featureNameLock = new object();
        public void SetFriendlyName(Guid id, string name)
        {
            lock (featureNameLock)
            {
                featureNames[id] = name;
            }
        }
        internal static string GetFriendlyName(Guid id)
        {
            lock (featureNameLock)
            {
                string name;
                if (featureNames.TryGetValue(id, out name)) return name;
                return null;
            }
        }

        private ICollection<KeyValuePair<string, Guid>> GetFeatureDomainPairsWithState(FeatureState state)
        {
            var l = new List<KeyValuePair<string, Guid>>();
            foreach (string domain in featureStates.Keys)
            {
                foreach (var p in featureStates[domain])
                {
                    if (p.Value == state) l.Add(new KeyValuePair<string, Guid>(domain, p.Key));
                }
            }
            return l;
        }

        private Dictionary<string, Dictionary<Guid, FeatureState>> featureStates = new Dictionary<string, Dictionary<Guid, FeatureState>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<Guid>> pendingFeatures = new Dictionary<string, List<Guid>>(StringComparer.OrdinalIgnoreCase);
        private object lockFeatures = new object();
        /// <summary>
        /// Plugins should use Methods.NotifyUse instead of directly using ILicenseServce, as it includes basic integrity verification
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="feature"></param>
        /// <param name="featureDisplayName"></param>
        public void NotifyUse(string domain, Guid feature)
        {
            domain = DomainUtility.NormalizeDomain(domain);

            FeatureState state;
            //Find out what the state of this feature is in the local cache, or initialize to Pending if unknown.
            lock (lockFeatures)
            {

                Dictionary<Guid, FeatureState> features;
                if (!featureStates.TryGetValue(domain, out features))
                {
                    features = new Dictionary<Guid, FeatureState>();
                    featureStates[domain] = features;
                }
                if (!features.TryGetValue(feature, out state))
                {
                    features[feature] = state = FeatureState.Pending;

                    //Add to shortlist pendingFeatures
                    List<Guid> forDomain;
                    if (!pendingFeatures.TryGetValue(domain, out forDomain))
                    {
                        forDomain = new List<Guid>();
                        pendingFeatures[domain] = forDomain;
                    }
                    forDomain.Add(feature);
                }
            }
            //Mark the request if feature license invalid. We'll deal with it later.
            if (state == FeatureState.Rejected)
            {
                HttpContext.Current.Items[RejectionKey] = feature;
            }

            PingBackgroundVerification();
        }

        private DateTime lastScheduledVerification = DateTime.MinValue;
        private DateTime lastEndedVerification = DateTime.MinValue;
        private object lockStart = new object();

        private void PingBackgroundVerification()
        {
            var now = DateTime.UtcNow;
            if (lastEndedVerification >= lastScheduledVerification && lastScheduledVerification + VerificationInterval < now && lastEndedVerification + VerificationInterval < now)
            {
                lock (lockStart)
                {
                    if (lastScheduledVerification + VerificationInterval > now) return; //Exit from race condition.
                    lastScheduledVerification = now;
                    ThreadPool.QueueUserWorkItem(Verify); //If it failed to queue, we'll get it next time.
                }
            }
        }


        private object singleThreadedVerify = new object();
        /// <summary>
        /// Runs on a thread pool thread periodically.
        /// </summary>
        /// <param name="o"></param>
        private void Verify(object o)
        {
            lock (singleThreadedVerify)
            {
                try
                {
                    //No verification of license store needed; they can be replaced by user
                    ILicenseStore s = config.Plugins.GetOrInstall<ILicenseStore>(new LicenseStore());
                    //Get all encrypted licenses and 
                    // Decrypt them, grouping by normalized domain name
                    var licenses = RemovedExpired(new LicenseDecrypter().DecryptAll(PublicKeyXml, s.GetLicenses()));
                    
                    //Fix all possible pendingFeatures using existing license data.
                    UpdateFeatureStatus(licenses);

                    if (pendingFeatures.Count > 0)
                    {
                        //Send off https request based on appId and pendingFeatures pairs.
                        //Get List<byte> in return.


                        var requestLicenses = new List<byte[]>();

                        string appStr = config.get("licenses.auto.appId", null);
                        if (!string.IsNullOrEmpty(appStr))
                        {
                            requestLicenses = new LicenseDownloader().RequestLicenses(LicensingUrl, new Guid(appStr), pendingFeatures);

                            var newLicenses = new LicenseDecrypter().DecryptAll(PublicKeyXml, requestLicenses);

                            if (newLicenses.Count > 0)
                            {
                                //Update cached feature statuses
                                UpdateFeatureStatus(newLicenses);

                                s.SetLicenses(ExportLicenses(Merge(licenses, newLicenses)));
                            }
                        }                        
                    }
                }
                finally
                {
                    lastEndedVerification = DateTime.UtcNow;
                }
            }
        }

        private List<KeyValuePair<string, byte[]>> ExportLicenses(Dictionary<string, List<DomainLicense>> licenses)
        {
            List<KeyValuePair<string, byte[]>> n = new List<KeyValuePair<string, byte[]>>();
            foreach (string key in licenses.Keys)
            {
                if (licenses[key] != null)
                {
                    foreach (var d in licenses[key])
                    {
                        n.Add(new KeyValuePair<string, byte[]>(d.GetShortDescription(), d.Encrypted));
                    }
                }
            }
            return n;

        }
        /// <summary>
        /// Merges b on top of a and returns the result without modifying either.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private Dictionary<string, List<DomainLicense>> Merge(Dictionary<string, List<DomainLicense>> a, Dictionary<string, List<DomainLicense>> b)
        {
            var n = new Dictionary<string, List<DomainLicense>>(a);
            foreach (string key in b.Keys)
            {
                List<DomainLicense> existingList;
                a.TryGetValue(key, out existingList);
                if (existingList == null) n[key] = b[key];
                else if (b[key] != null) existingList.AddRange(b[key]);
            }
            return n;
        }


        private Dictionary<string, List<DomainLicense>> RemovedExpired(Dictionary<string, List<DomainLicense>> licenses)
        {
            DateTime now = DateTime.UtcNow;
            var d = new Dictionary<string, List<DomainLicense>>(licenses.Comparer);
            foreach (string s in licenses.Keys)
            {
                List<DomainLicense> remaining = null;
                if (licenses[s] == null) continue;
                foreach (DomainLicense l in licenses[s])
                {
                    if (l.Expires > now)
                    {
                        if (remaining == null) remaining = new List<DomainLicense>();
                        remaining.Add(l);
                    }
                }
                if (remaining != null) d[s] = remaining;
            }
            return d;
        }

        /// <summary>
        /// Updates the 'pendingFeatures' domain->feature[] map based on the provided collection of licenses.
        /// </summary>
        /// <param name="licenses"></param>
        private void UpdateFeatureStatus(Dictionary<string, List<DomainLicense>> licenses)
        {
            DateTime now = DateTime.UtcNow;
            //Iterate through pendingFeatures and resolve them.
            lock (lockFeatures)
            {
                //Loop through pendingFeature domain names
                var pendingDomains = new List<string>(pendingFeatures.Keys);
                foreach (string domain in pendingDomains)
                {
                    //Skip domain names that don't have matching licenses
                    List<DomainLicense> forDomain;
                    if (!licenses.TryGetValue(domain, out forDomain)) continue;

                    //Loop through mending features for domain
                    var domainPendingFeatures = pendingFeatures[domain];
                    var originalFeatures = new List<Guid>(domainPendingFeatures);
                    foreach (var featureId in originalFeatures)
                    {
                        //Loop through licenses for the domain
                        foreach (var l in forDomain)
                        {
                            //If the license hasn't expired and has a matching feature,
                            // remove the feature from pendingFeatures and update featureStates
                            if (l.Expires > now && l.Features.Contains(featureId))
                            {
                                featureStates[domain][featureId] = FeatureState.Enabled;
                                domainPendingFeatures.Remove(featureId);
                                if (domainPendingFeatures.Count == 0) pendingFeatures.Remove(domain);
                            }
                        }
                    }


                }
            }
        }

        /// <summary>
        /// Generates a key 2048-bit keypair and returns the xml fragment containing it
        /// </summary>
        /// <returns></returns>
        internal static string GenerateKeyPairXml()
        {
            using (var r = new RSACryptoServiceProvider(2048))
                return r.ToXmlString(true);
        }
        /// <summary>
        /// Strips the private information from the given key pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        internal static string StripPrivateKey(string pair)
        {
            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.FromXmlString(pair);
                return r.ToXmlString(false);
            }
        }


        public IPlugin Install(Config c)
        {
            this.config = c;
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            return false;
        }

        private Dictionary<Guid, long> tracker;
        private object lockTracker = new object();

        /// <summary>
        /// Verifies the authenticty of this ILicenseService witht the caller
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public long VerifyAuthenticity(Guid feature)
        {
            lock (lockTracker)
            {
                long seed = feature.ToString().GetHashCode();
                long auth = seed;
                if (tracker == null) tracker = new Dictionary<Guid, long>();
                if (!tracker.TryGetValue(feature, out auth)) auth = seed;
                long temp = auth;
                tracker[feature] = auth + 1;
                return temp;
            }
        }
    }
    /// <summary>
    /// Implementation of ILicenseStore that uses a file in App_Data
    /// </summary>
    internal class LicenseStore : ILicenseStore, IPlugin, IIssueReceiver
    {

        Config c;
        public IPlugin Install(Config c)
        {
            this.c = c;
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            return false;
        }

        internal List<byte[]> LicensesFromXml(XmlElement xml)
        {
            return LicensesFromXml(new Node(xml, this));
        }

        internal List<byte[]> LicensesFromXml(Node xml)
        {
            List<byte[]> licenses = new List<byte[]>();
            var n = xml;
            foreach (var l in n.childrenByName("license"))
            {
                if (l.TextContents == null) continue;
                licenses.Add(Convert.FromBase64String(l.TextContents.Trim()));

            }
            return licenses;
        }

        private List<byte[]> LicensesFromFile(string name)
        {
            if (!File.Exists(name)) return new List<byte[]>();
            var d = new XmlDocument();
            d.Load(name);
            return LicensesFromXml(d.DocumentElement);

        }
        private void LicensesToFile(string name, ICollection<KeyValuePair<string, byte[]>> licenses)
        {
            var d = new XmlDocument();
            d.AppendChild(LicensesToXml(licenses).ToXmlElement(d));
            d.Save(name);
        }
        private Node LicensesToXml(ICollection<KeyValuePair<string, byte[]>> licenses)
        {
            Node n = new Node("licenses");
            foreach (var p in licenses)
            {
                var l = new Node("license");
                var desc = new Node("description");
                desc.TextContents = p.Key;
                l.Children.Add(desc);
                l.TextContents = Convert.ToBase64String(p.Value);
                n.Children.Add(l);
            }
            return n;
        }

        private List<byte[]> webConfigLicenses;
        /// <summary>
        /// Provides cached access to web.config licenses.
        /// </summary>
        /// <returns></returns>
        private List<byte[]> GetWebConfigLicenses()
        {
            if (webConfigLicenses == null)
                webConfigLicenses = LicensesFromXml(c.getNode("licenses"));
            return new List<byte[]>(webConfigLicenses); ;
        }

        private string _filename = null;
        private string GetLicenseFilename()
        {
            if (_filename == null)
                _filename = HostingEnvironment.MapPath("~/App_Data/imazen-resizer-licenses-" + c.get("licenses.auto.appId", "unconfigured") + ".xml");
            return _filename;
        }

        private List<byte[]> storedLicenses;

        private List<byte[]> GetStoredLicenses()
        {
            if (storedLicenses == null)
            {
                storedLicenses = LicensesFromFile(GetLicenseFilename());
            }
            return new List<byte[]>(storedLicenses);
        }

        private object licenseLock = new object();
        public ICollection<byte[]> GetLicenses()
        {
            lock (licenseLock)
            {
                var licenses = GetWebConfigLicenses();

                //Read from ~/App_Data/imazen-licenses-guid.txt 
                licenses.AddRange(GetStoredLicenses());
                return licenses;
            }
        }

        public void SetLicenses(ICollection<KeyValuePair<string, byte[]>> licenses)
        {
            lock (licenseLock)
            {
                //Ignore licenses that are already in Web.config; save the rest to a file
                var remainder = new List<KeyValuePair<string, byte[]>>(licenses);
                //Remove intersection with web.config
                var web = GetWebConfigLicenses();
                foreach (var l in web)
                {
                    for (int i = 0; i < remainder.Count; i++)
                    {
                        if (ArraysMatch(remainder[i].Value, l))
                        {
                            remainder.RemoveAt(i);
                            break;
                        }
                    }
                }
                //Save remainder to disk
                LicensesToFile(GetLicenseFilename(), remainder);
                //Flatten and cache 
                List<byte[]> flattened = new List<byte[]>();
                foreach (var p in remainder) flattened.Add(p.Value);
                storedLicenses = flattened;
            }
        }
        internal bool ArraysMatch(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        public void AcceptIssue(IIssue i)
        {
            //Ignore duplicate errors loading xml
        }
    }
}
