using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.BusinessLogic.LicenseValidation
{
    public static class LicenseValidator
    {
        public static string ValidateLicense(DomainValidation validationRequest)
        {
            return "";
        }
    }

    public class DomainValidation
    {
        public DomainValidation(string domain, Guid[] features)
        {
            this.Domain = domain;
            this.Features = features;
        }

        public string Domain { get; set; }
        public Guid[] Features { get; set; }
    }



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class licenseRequest
    {

        private string appIdField;

        private licenseRequestDomain[] domainsField;

        /// <remarks/>
        public string appId
        {
            get
            {
                return this.appIdField;
            }
            set
            {
                this.appIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("domain", IsNullable = false)]
        public licenseRequestDomain[] domains
        {
            get
            {
                return this.domainsField;
            }
            set
            {
                this.domainsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class licenseRequestDomain
    {

        private string[] featureField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("feature")]
        public string[] feature
        {
            get
            {
                return this.featureField;
            }
            set
            {
                this.featureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }


}
