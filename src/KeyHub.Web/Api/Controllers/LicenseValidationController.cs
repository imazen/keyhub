using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Core.Logging;
using KeyHub.Common;
using KeyHub.Core.UnitOfWork;
using KeyHub.Data;
using KeyHub.Data.ApplicationIssues;
using KeyHub.Model;
using KeyHub.Web.Api.Controllers.LicenseValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for validating licenses
    /// </summary>
    public class LicenseValidationController : ApiController
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly IApplicationIssueUnitOfWork applicationIssueUnitOfWork;
        private readonly ILoggingService loggingService;
        private readonly ILicenseValidator licenseValidator;

        public LicenseValidationController(IDataContextFactory dataContextFactory, IApplicationIssueUnitOfWork applicationIssueUnitOfWork,
                                           ILoggingService loggingService, ILicenseValidator licenseValidator)
        {
            this.dataContextFactory = dataContextFactory;
            this.applicationIssueUnitOfWork = applicationIssueUnitOfWork;
            this.loggingService = loggingService;
            this.licenseValidator = licenseValidator;
        }
        
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
        public HttpResponseMessage Post([FromBody]LicenseValidationRequest request)
        {
            try
            {
                IEnumerable<DomainValidationResult> domainValidationResults = licenseValidator.Validate(request.AppId, ToDomainValidationList(request));

                string domainValidationString = domainValidationResults != null ? Serialize(domainValidationResults) : string.Empty;

                return new HttpResponseMessage
                {
                    Content = new StringContent(domainValidationString, Encoding.UTF8, "text/html")
                };
            }
            catch (Exception e)
            {
                loggingService.Log(e.ToString(), LogTypes.Error);

                return new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty, Encoding.UTF8, "text/html")
                };
            }
        }

        /// <summary>
        /// Converts license validation request to combination of domainName/featureCode
        /// </summary>
        /// <param name="licenseValidationRequest"></param>
        /// <returns></returns>
        private static IEnumerable<DomainValidation> ToDomainValidationList(LicenseValidationRequest licenseValidationRequest)
        {
            return (from licenseValidationRequestDomain in licenseValidationRequest.Domains
                    from featureCode in licenseValidationRequestDomain.Feature
                    select new DomainValidation(licenseValidationRequestDomain.name, featureCode)).ToList();
        }

        /// <summary>
        /// Serializes validation result
        /// </summary>
        /// <param name="domainValidationResults">Validation result</param>
        /// <returns>Serialized string with encoded values</returns>
        /// <example>
        /// <licenses>
        /// <license>base64-encoded-license</license>
        /// <license>base64-encoded-license</license>
        /// <license>base64-encoded-license</license>
        /// <license>base64-encoded-license</license>
        /// <license>base64-encoded-license</license>
        /// </licenses>
        /// </example>
        private static string Serialize(IEnumerable<DomainValidationResult> domainValidationResults)
        {
            var xLicenses = new XElement(Constants.LicensesTag);

            foreach (var domainValidationResult in domainValidationResults)
            {
                var value = domainValidationResult.DomainLicense.SerializeUnencrypted();
                var valueElement = new XElement(Constants.LicenseValueTag, Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));

                var signature = SignData(value, domainValidationResult.KeyBytes);
                var signatureElement = new XElement(Constants.LicenseSignatureTag, Convert.ToBase64String(signature));
                
                var licenseElement = new XElement(Constants.LicenseTag, valueElement, signatureElement);
                xLicenses.Add(licenseElement);
            }

            return xLicenses.ToString();
        }

        /// <summary>
        /// Get Base64 string RSA signature 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keyBytes">Blob of RSA private key</param>
        /// <returns>Encrypted text</returns>
        public static byte[] SignData(string text, byte[] keyBytes)
        {
            using (var r = new RSACryptoServiceProvider(2048, new CspParameters() { Flags = CspProviderFlags.NoPrompt | CspProviderFlags.CreateEphemeralKey }))
            {
                try
                {
                    r.ImportCspBlob(keyBytes);
                    return r.SignData(Encoding.UTF8.GetBytes(text), new SHA256Managed());
                }
                finally
                {
                    r.PersistKeyInCsp = false;
                }
            }
        }
    }
}