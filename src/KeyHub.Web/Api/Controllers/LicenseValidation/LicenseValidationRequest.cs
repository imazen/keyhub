using System;

namespace KeyHub.Web.Api.Controllers.LicenseValidation
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "licenseRequest", Namespace = "", IsNullable = false)]
    public partial class LicenseValidationRequest
    {

        private Guid appIdField;

        private LicenseValidationRequestDomain[] domainsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("appId")]
        public Guid AppId
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
        /// 
        [System.Xml.Serialization.XmlArrayAttribute("domains", IsNullable = false)]
        [System.Xml.Serialization.XmlArrayItemAttribute("domain", IsNullable = false)]
        public LicenseValidationRequestDomain[] Domains
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
    public partial class LicenseValidationRequestDomain
    {

        private Guid[] featureField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("feature")]
        public Guid[] Feature
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