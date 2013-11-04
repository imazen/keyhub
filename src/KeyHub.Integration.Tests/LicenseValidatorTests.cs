﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.BusinessLogic.BusinessRules;
using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Client;
using KeyHub.Core.Logging;
using KeyHub.Data;
using KeyHub.Data.ApplicationIssues;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Moq;
using Newtonsoft.Json;
using Xunit;
using DomainLicense = KeyHub.Client.DomainLicense;

namespace KeyHub.Integration.Tests
{
    public class LicenseValidatorTests
    {
        [Fact]
        [CleanDatabase]
        public void CanValidateLicense()
        {
            var domain = "lol1.lo2.foobar.com";

            var scenario = new LicenseValidationScenario();

            var identity = new Mock<IIdentity>(MockBehavior.Loose);
            var loggingService = new Mock<ILoggingService>(MockBehavior.Loose);
            var applicationIssuesUnitOfWork = new Mock<IApplicationIssueUnitOfWork>(MockBehavior.Loose);

            using (var dataContextFactory = new CheckedDataContextFactory(identity.Object))
            {
                var validator = new LicenseValidator(dataContextFactory, loggingService.Object,
                    applicationIssuesUnitOfWork.Object);

                var result = validator.Validate(scenario.AppKey, new[] { new DomainValidation(domain, scenario.FeatureCode) }).Single();

                Assert.Equal(result.DomainLicense.Domain, domain);
                Assert.Contains(scenario.FeatureCode, result.DomainLicense.Features);
            }
        }
        
        [Fact]
        [CleanDatabase]
        public void CanValidateLicenseRemotely()
        {
            var domain = "foobar.com";

            var scenario = new LicenseValidationScenario();

            using (var site = new KeyHubWebDriver())
            {
                var licensingUrl = site.UrlFor("/api/LicenseValidation");

                var licensesAndSignature = new LicenseDownloader().RequestLicenses(licensingUrl, scenario.AppKey, new Dictionary<string, List<Guid>>()
                {
                    {domain, new List<Guid>() {scenario.FeatureCode}}
                });

                var newLicenses = new LicenseDeserializer().DeserializeAll(scenario.PublicKeyXml, licensesAndSignature);

                DomainLicense license = newLicenses["foobar.com"].Single();

                Assert.Equal(license.Domain, domain);
                Assert.Contains(scenario.FeatureCode, license.Features);
            }
        }
    }
}
