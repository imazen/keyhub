using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Xml;
using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Web.Api.Controllers.LicenseValidation;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for validating licenses
    /// </summary>
    public class LicenseValidationController : ApiController
    {
        /// <summary>
        /// License validation post
        /// </summary>
        /// <param name="request">LicenseValidationRequest to validate</param>
        /// <returns>LicenseValidationResult with validation result</returns>
        /// <example>
        ///     POST http://localhost:63436/api/licensevalidation/ HTTP/1.2
        ///     User-Agent: Fiddler
        ///     Host: localhost:63436
        ///     Content-Length: 357
        ///     Content-Type: application/xml
        ///     <?xml version="1.0" encoding="utf-8"?>
        ///     <LicenseValidationRequest>
        ///         <AppId>{guid}</AppId>
        ///         <Domains>
        ///             <Domain name="microsoft.com">
        ///                 <Feature>{guid}</Feature>
        ///                 <Feature>{guid}</Feature>
        ///             </Domain>
        ///             <Domain name="microsoft.com">
        ///                 <Feature>{guid}</Feature>
        ///                 <Feature>{guid}</Feature>
        ///             </Domain>
        ///         </Domains>
        ///     </LicenseValidationRequest>
        /// </example> 
        public LicenseValidationResult Post([FromBody]LicenseValidationRequest request)
        {
            var domainsToValidate = (from x in request.Domains select new DomainValidation(x.name, x.Feature)).ToList();
            
            LicenseValidator.ValidateLicense(request.AppId, domainsToValidate);

            return new LicenseValidationResult();
        }
    }


}