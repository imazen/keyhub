using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Core.Logging;
using KeyHub.Runtime;
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

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for validating licenses
    /// </summary>
    public class LicenseValidationController : ApiController
    {
        #region String Constants (tags)

        private const string LicensesOpenTag = "<licenses>";
        private const string LicensesCloseTag = "</licenses>";
        private const string LicenseOpenTag = "<license>";
        private const string LicenseCloseTag = "</license>";

        #endregion

        /// <summary>
        /// Path to public key for encrypting by RSA validation result
        /// </summary>
        private const string PublicKeyPath = @"~/App_Data/RsaKeys/public.xml";

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
            var domainsToValidate = (from x in request.Domains select new DomainValidation(x.name, x.Feature)).ToList();

            try
            {
                IEnumerable<DomainValidationResult> domainValidationResults = LicenseValidator.ValidateLicense(request.AppId, domainsToValidate);

                string domainValidationString = domainValidationResults != null ? Serialize(domainValidationResults) : string.Empty;

                if (domainValidationResults == null)
                    throw new Exception("Application not found");

                return new HttpResponseMessage
                {
                    Content = new StringContent(domainValidationString, Encoding.UTF8, "text/html")
                };
            }
            catch (Exception e)
            {
                LogContext.Instance.Log(e.ToString(), LogTypes.Error);

                return new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty, Encoding.UTF8, "text/html")
                };
            }
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
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(LicensesOpenTag);
            foreach (var domainValidationResult in domainValidationResults)
            {
                stringBuilder
                    .Append(LicenseOpenTag)
                    .Append(Encrypt(domainValidationResult.Serialize()))
                    .AppendLine(LicenseCloseTag);
            }
            stringBuilder.AppendLine(LicensesCloseTag);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get Base64 string encrypted RSA 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Encrypted text</returns>
        private static string Encrypt(string text)
        {
            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.FromXmlString(File.ReadAllText(HttpContext.Current.Server.MapPath(PublicKeyPath)));
                return Convert.ToBase64String(r.Encrypt(Encoding.UTF8.GetBytes(text), false));
            }
        }
    }


}