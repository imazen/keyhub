using System;
using System.Linq;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Tests.TestCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class ExampleTest
    {
        private TestContext testContextInstance;
        private Mock<IDataContext> datacontext;

        /// <summary>
        /// Initialize all mockin on TestInitialize
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            datacontext = new Mock<IDataContext>();
            datacontext.Setup(x => x.Countries).Returns(new FakeDbSet<Country> { new Country { CountryCode = "LU", CountryName = "Lucracountry" } });
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
            var country = (from x in datacontext.Object.Countries select x).FirstOrDefault();
            TestContext.WriteLine(String.Format("Country: {0}", country.CountryName));

            Assert.AreEqual("Lucracountry", country.CountryName);
        }

        /// <summary>
        /// Mock countries collection from DataContext
        /// </summary>
        [TestMethod]
        public void MockCountries()
        {
            var countries = (from x in datacontext.Object.Countries select x);
            TestContext.WriteLine(String.Format("Countries: {0}, name: {1}", countries.Count(), countries.FirstOrDefault().CountryName));

            Assert.AreEqual(1, countries.Count());
            Assert.AreEqual("Lucracountry", countries.FirstOrDefault().CountryName);
        }
    }
}
