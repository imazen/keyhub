using System;
using System.Collections.Generic;
using System.Linq;
using KeyHub.Data;
using KeyHub.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class ExampleTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Initialize all mockin on TestInitialize
        /// </summary>
        /// <remarks>
        /// Mock countries collection from DataContext
        /// http://www.telerik.com/help/justmock/advanced-usage-future-mocking.html
        /// </remarks>
        /// <remarks>
        /// Mock country name
        /// Uses IgnoreInstance to apply future mocking
        /// http://www.telerik.com/help/justmock/advanced-usage-entity-framework-mocking.html
        /// </remarks>
        [TestInitialize]
        public void Initialize()
        {
            var mockContext = new DataContext();
            Mock.Arrange(() => mockContext.Countries).IgnoreInstance().ReturnsCollection(new List<Country> { new Country { CountryCode = "LU", CountryName = "Lucracountry" } });

            var countryMock = Mock.Create<Country>();
            Mock.Arrange(() => countryMock.CountryName).IgnoreInstance().Returns("Lucracountry mock");
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        /// <summary>
        /// Mock country name
        /// </summary>
        [TestMethod]
        public void MockCountryInstance()
        {
            using (var context = new DataContext())
            {
                var country = (from x in context.Countries select x).FirstOrDefault();
                TestContext.WriteLine(String.Format("Country: {0}", country.CountryName));

                Assert.AreEqual("Lucracountry mock", country.CountryName);
            }
        }

        /// <summary>
        /// Mock countries collection from DataContext
        /// </summary>
        [TestMethod]
        public void MockCountries()
        {
            using (var context = new DataContext())
            {
                var countries = (from x in context.Countries select x);
                TestContext.WriteLine(String.Format("Countries: {0}, name: {1}", countries.Count(), countries.FirstOrDefault().CountryName));

                Assert.AreEqual(1, countries.Count());
                Assert.AreEqual("Lucracountry mock", countries.FirstOrDefault().CountryName);
            }
        }
    }
}
