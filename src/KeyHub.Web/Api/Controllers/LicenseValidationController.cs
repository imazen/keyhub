using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Xml;
using KeyHub.BusinessLogic.LicenseValidation;

namespace KeyHub.Web.Api.Controllers
{
    public class LicenseValidationController : ApiController
    {



        /// <summary>
        /// Http verification post
        /// </summary>
        /// <param name="licenseRequest">Transaction to create</param>
        /// <returns>TransactionResult containing success and optional errormessage</returns>
        public licenserequest Post([FromBody] licenserequest request)
        {
            //var test = new DomainValidation(validationRequest.licenseRequest.appId, )
            //LicenseValidator.ValidateLicense();

            return request;
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class licenserequest
    {

        private string appIdField;

        private licenserequestDomains domainsField;

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
        public licenserequestDomains domains
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
    public partial class licenserequestDomains
    {

        private licenserequestDomainsDomain domainField;

        /// <remarks/>
        public licenserequestDomainsDomain domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class licenserequestDomainsDomain
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