using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyHub.Web;
using KeyHub.Web.Controllers;
using Telerik.JustMock;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            //Prepare HttpContext and Principal
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            // User is logged in
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity("admin"),
                new string[0]
                );

            // Mock user instance
            var userMock = Mock.Create<User>();
            Mock.Arrange(() => userMock.IsVendorAdmin).IgnoreInstance().Returns(true);
            Mock.Arrange(() => userMock.IsSystemAdmin).IgnoreInstance().Returns(true);
        }

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }
    }
}
