using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.BusinessLogic.BusinessRules;
using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Core.Logging;
using KeyHub.Data;
using KeyHub.Data.ApplicationIssues;
using KeyHub.Integration.Tests.TestSetup;
using KeyHub.Model;
using Moq;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class LicenseValidatorTests
    {
        [Fact]
        [CleanDatabase]
        public void CanValidateLicense()
        {
            var appKey = Guid.NewGuid();
            var featureGuid = Guid.NewGuid();
            var domain = "foobar.com";

            using (var dataContext = new DataContext())
            {
                var customerApp = new CustomerApp()
                {
                    ApplicationName = "test CustomerApp"
                };

                dataContext.CustomerApps.Add(customerApp);
                dataContext.SaveChanges();

                Console.WriteLine("id" + customerApp.CustomerAppId);

                dataContext.CustomerAppKeys.Add(new CustomerAppKey()
                {
                    CustomerAppId = customerApp.CustomerAppId,
                    AppKey = appKey
                });

                dataContext.SaveChanges();
            }

            var identity = new Mock<IIdentity>(MockBehavior.Loose);
            var loggingService = new Mock<ILoggingService>(MockBehavior.Loose);
            var applicationIssuesUnitOfWork = new Mock<IApplicationIssueUnitOfWork>(MockBehavior.Loose);

            using (var dataContextFactory = new CheckedDataContextFactory(identity.Object))
            {
                var validator = new LicenseValidator(dataContextFactory, loggingService.Object,
                    applicationIssuesUnitOfWork.Object);

                var result = validator.Validate(appKey, new[] {new DomainValidation(domain, featureGuid)}).Single();

                Assert.Equal(result.DomainName, "foorbar.com");
                Assert.Contains(featureGuid, result.Features);
            }
        }
    }
}
