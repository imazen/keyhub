using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using KeyHub.BusinessLogic.BusinessRules.LicenseValidation;
using KeyHub.BusinessLogic.LicenseValidation;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace KeyHub.Tests.BusinessLogic.LicenseValidation
{
    [TestClass]
    public class MaxDomainsCountTest
    {
        // ReSharper disable InconsistentNaming

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private DataContext context;
        private LicenseValidator licenseValidator;

        private string domainName1;
        private Guid appKey;
        private List<DomainValidation> domainValidations;

        private Feature feature1;
        private Feature feature2;
        private PrivateKey privateKey1;
        private SKU sku1;
        private License license1;
        private CustomerAppKey customerAppKey1;
        private CustomerApp customerApp1;
        private List<DomainLicense> domainLicenses;
        private DomainLicense domainLicense1;

        private List<DomainValidationResult> domainValidationResults;

        // ReSharper restore InconsistentNaming
            
        [TestInitialize]
        public void Initialize()
        {
            ForceDataContextIntercepting();

            IDataContextFactory factory = null;
            context = new DataContext();
            var test = LicenseValidator.ValidateLicense(factory, new Guid(), null);

            domainName1 = "domain1";
            appKey = GuidTestData.Create(100);
            domainValidations = new List<DomainValidation>
            {
                new DomainValidation(domainName1, GuidTestData.Create(1)),
                new DomainValidation(domainName1, GuidTestData.Create(2))
            };

            feature1 = FeatureTestData.Create(GuidTestData.Create(1));
            feature2 = FeatureTestData.Create(GuidTestData.Create(2));
            privateKey1 = PrivateKeyTestData.Create();
            sku1 = SkuTestData.Create(privateKey1, feature1, feature2);
            license1 = LicenseTestData.Create(sku1);
            customerAppKey1 = CustomerAppKeyTestData.Create(appKey);
            customerApp1 = CustomerAppTestData.Create(customerAppKey1, license1);
            domainLicenses = new List<DomainLicense> { new DomainLicense() };

            IQueryable<DomainLicense> queryable = new List<DomainLicense>().AsQueryable();
            Mock.Arrange(() => queryable.Include(Arg.IsAny<Expression<Func<DomainLicense, IEnumerable<Feature>>>>()))
                .IgnoreInstance()
                .ReturnsCollection(domainLicenses);
        }

        // ReSharper disable ImplicitlyCapturedClosure

        [TestMethod]
        public void AddDomainLicenseWhenMaxDomainsCountNotAssigned()
        {
            MockSku();

            Mock.Arrange(() => context.CustomerAppKeys)
                .IgnoreInstance()
                .ReturnsCollection(customerApp1.CustomerAppKeys);

            Mock.Arrange(() => context.DomainLicenses)
                .IgnoreInstance()
                .ReturnsCollection(domainLicenses);

            Mock.Arrange(() => context.Licenses)
                .IgnoreInstance()
                .ReturnsCollection(new List<License> { license1 });

            Mock.Arrange(() => context.DomainLicenses.Add(Arg.Matches((DomainLicense d) => d.DomainName == domainName1)))
                .IgnoreInstance()
                .DoInstead((DomainLicense d) => domainLicense1 = d);

            Mock.Arrange(() => context.SaveChanges())
                .IgnoreInstance()
                .DoInstead(() => context.ValidateModelItem(domainLicenses[0]));

            Mock.Arrange(() => licenseValidator.DeleteExpiredDomainLicenses())
                .IgnoreInstance()
                .DoNothing();

            ActAndAssert(AssertDomainsNames);
        }

        [TestMethod]
        public void AddDomainLicenseWhenMaxDomainsCountSatisfied()
        {
            sku1.MaxDomains = 1;

            Mock.Arrange(() => context.CustomerAppKeys)
                .IgnoreInstance()
                .ReturnsCollection(customerApp1.CustomerAppKeys);

            Mock.Arrange(() => context.DomainLicenses)
                .IgnoreInstance()
                .ReturnsCollection(domainLicenses);

            Mock.Arrange(() => context.Licenses)
                .IgnoreInstance()
                .ReturnsCollection(new List<License> { license1 });

            Mock.Arrange(() => context.DomainLicenses.Add(Arg.Matches((DomainLicense d) => d.DomainName == domainName1 && d.AutomaticlyCreated)))
                .IgnoreInstance()
                .DoInstead((DomainLicense d) => domainLicenses.Add(d));

            Mock.Arrange(() => context.SaveChanges())
                .IgnoreInstance()
                .DoInstead(() => context.ValidateModelItem(domainLicenses[0]));

            Mock.Arrange(() => licenseValidator.DeleteExpiredDomainLicenses())
                .IgnoreInstance()
                .DoNothing();


            ActAndAssert(AssertDomainsNames);
        }

        [TestMethod]
        public void NotAddDomainLicenseWhenSameDomainLicense()
        {
            sku1.MaxDomains = 1;
            domainLicense1 = DomainLicenseTestData.Create(domainName1, license1);
            domainLicenses.Add(domainLicense1);

            Mock.Arrange(() => context.CustomerAppKeys)
                .IgnoreInstance()
                .ReturnsCollection(customerApp1.CustomerAppKeys);

            Mock.Arrange(() => context.DomainLicenses)
                .IgnoreInstance()
                .ReturnsCollection(domainLicenses);

            Mock.Arrange(() => context.Licenses)
                .IgnoreInstance()
                .ReturnsCollection(new List<License> { license1 })
                .OccursNever();

            Mock.Arrange(() => context.DomainLicenses.Add(Arg.Matches((DomainLicense d) => d.DomainName == domainName1)))
                .IgnoreInstance()
                .OccursNever();

            Mock.Arrange(() => context.SaveChanges())
                .IgnoreInstance()
                .OccursNever();

            Mock.Arrange(() => licenseValidator.DeleteExpiredDomainLicenses())
                .IgnoreInstance()
                .DoNothing();

            ActAndAssert(AssertDomainsNames);
        }

        [TestMethod]
        public void NotAddDomainLicenseWhenMaxDomainsViolation()
        {
            const string domainName2 = "domain2";
            domainValidations[1].DomainName = domainName2;
            sku1.MaxDomains = 1;
            DomainLicense domainLicense1 = null;
            DomainLicense domainLicense2 = DomainLicenseTestData.Create(domainName2, license1);
            domainLicenses.Add(domainLicense2);

            Mock.Arrange(() => context.CustomerAppKeys)
                .IgnoreInstance()
                .ReturnsCollection(customerApp1.CustomerAppKeys);

            Mock.Arrange(() => context.DomainLicenses)
                .IgnoreInstance()
                .ReturnsCollection(domainLicenses);

            Mock.Arrange(() => context.Licenses)
                .IgnoreInstance()
                .ReturnsCollection(new List<License> { license1 });

            Mock.Arrange(() => context.DomainLicenses.Add(Arg.Matches<DomainLicense>(d => d.DomainName == domainName1)))
                .IgnoreInstance()
                .DoInstead<DomainLicense>(d => domainLicense1 = d);

            Mock.Arrange(() => context.SaveChanges(Arg.IsAny<Action<BusinessRuleValidationException>>()))
               .IgnoreInstance()
               .DoInstead<Action<BusinessRuleValidationException>>(validationFailedAction => SaveChangesForTesting(context, validationFailedAction));

            Mock.Arrange(() => context.SaveChanges())
               .IgnoreInstance()
               .DoInstead(() => context.ValidateModelItem(domainLicense1));

            Mock.Arrange(() => licenseValidator.OnValidationFailed(Arg.Matches<BusinessRuleValidationException>(e => Match(e, domainName1, license1.ObjectId))))
                .IgnoreInstance()
                .DoNothing()
                .OccursOnce();

            Mock.Arrange(() => licenseValidator.DeleteExpiredDomainLicenses())
                .IgnoreInstance()
                .DoNothing();

            ActAndAssert(AssertDomainsNamesExcept, domainName1);
        }

        // ReSharper restore ImplicitlyCapturedClosure

        private void MockSku()
        {
            Mock.Initialize<SKU>();

            Mock.Arrange(() => sku1.CalculateDomainIssueDate())
                .MustBeCalled();

            Mock.Arrange(() => sku1.CalculateAutoDomainExpiration())
                .MustBeCalled();
        }

        private void AssertDomainsNames()
        {
            List<string> domainNames = domainValidations.Select(x => x.DomainName).Distinct().ToList();
            Assert.AreEqual(domainNames.Count, domainValidationResults.Count);
            for (int i = 0; i < domainNames.Count; i++)
            {
                Assert.AreEqual(domainNames[i], domainValidationResults[i].DomainName);
            }
        }

        private void AssertDomainsNamesExcept(string domainName)
        {
            List<string> domainNames = domainValidations.Select(x => x.DomainName).Distinct().Where(x => x != domainName).ToList();
            Assert.AreEqual(domainNames.Count, domainValidationResults.Count);
            for (int i = 0; i < domainNames.Count; i++)
            {
                Assert.AreEqual(domainNames[i], domainValidationResults[i].DomainName);
            }
        }

        private void ActAndAssert(Action assertAction)
        {
            IDataContextFactory dataContextFactory = null;
            domainValidationResults = LicenseValidator.ValidateLicense(dataContextFactory, appKey, domainValidations).ToList();

            assertAction();

            Mock.AssertAll(context);
            Mock.AssertAll(licenseValidator);
            Mock.AssertAll(sku1);
        }

        private void ActAndAssert<T>(Action<T> assertAction, T parameter)
        {
            IDataContextFactory dataContextFactory = null;
            domainValidationResults = LicenseValidator.ValidateLicense(dataContextFactory, appKey, domainValidations).ToList();

            assertAction(parameter);

            Mock.AssertAll(context);
            Mock.AssertAll(licenseValidator);
        }

        /// <summary>
        /// .NET profiler can intercept a member only once during OnJITCompilationStarted and if it is invoked manually first 
        /// then prevents JustMock from intercepting it. 
        /// It is possible to pre-initialize the member using Mock.Initialize{T} to resolve this side effect
        /// </summary>
        private static void ForceDataContextIntercepting()
        {
            Mock.Initialize<DataContext>();
        }

        private static bool Match(BusinessRuleValidationException businessRuleValidationException, string domainName, Guid licenseId)
        {
            BusinessRuleValidationResult businessRuleValidationResult = businessRuleValidationException.ValidationResults.First();
            return businessRuleValidationResult.ErrorMessage == string.Format("MaxDomains violation: domain: {0}, licenseId: {1}", domainName, licenseId)
                   && businessRuleValidationResult.BusinessRuleName == new MaxDomainsCountRule().BusinessRuleName
                   && string.IsNullOrEmpty(businessRuleValidationResult.PropertyName);
        }

        /// <summary>
        /// Should be the same as real <code>DataContext::SaveChanges(Action{BusinessRuleValidationException})</code>
        /// In order to prevent System.Reflection.TargetInvocationException on mocked method invocation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="validationFailedAction"></param>
        /// <returns></returns>
        private static bool SaveChangesForTesting(DataContext context, Action<BusinessRuleValidationException> validationFailedAction)
        {
            try
            {
                try
                {
                    context.SaveChanges();
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    if (ex.InnerException is BusinessRuleValidationException)
                    {
                        throw ex.InnerException;
                    }

                    throw;
                }
                return true;
            }
            catch (BusinessRuleValidationException ex)
            {
                validationFailedAction(ex);
            }

            return false;
        }
    }
}
