using System.Security.Principal;
using System.Web.Mvc;
using KeyHub.Tests.TestCore;
using KeyHub.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyHub.Web.Controllers;
using Moq;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController controller;

        [TestInitialize]
        public void Initialize()
        {          
            var dataContextFactory = new FakeDataContextFactory();
            dataContextFactory.DataContext
                .Setup(x => x.GetUser(It.IsAny<IIdentity>()))
                .Returns(UserTestData.CreateSysAdmin());
            
            controller = new HomeController(dataContextFactory);
            controller.SetFakeControllerContext();
            controller.HttpContext.User = new GenericPrincipal(new GenericIdentity(""), new string[0]);
        }

        [TestMethod]
        public void IndexShouldReturnViewResult()
        {
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }
    }
}
