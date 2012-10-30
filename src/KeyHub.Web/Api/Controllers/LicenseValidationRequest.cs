using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeyHub.Web.Api.Controllers
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class LicenseValidationRequest
    {

        private Guid appIdField;

        private LicenseValidationRequestDomain[] domainsField;

        /// <remarks/>
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
        [System.Xml.Serialization.XmlArrayItemAttribute("Domain", IsNullable = false)]
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
        [System.Xml.Serialization.XmlElementAttribute("Feature")]
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