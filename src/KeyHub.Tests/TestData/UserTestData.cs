using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;
using Moq;

namespace KeyHub.Tests.TestData
{
    public static class UserTestData
    {
        public static User CreateSysAdmin()
        {
            var user = new Mock<User>();
            user.Setup(x => x.IsSystemAdmin).Returns(true);
            user.Setup(x => x.IsVendorAdmin).Returns(true);
            return user.Object;
        }

        public static User CreateAnonymous()
        {
            var user = new Mock<User>();
            user.Setup(x => x.IsSystemAdmin).Returns(false);
            user.Setup(x => x.IsVendorAdmin).Returns(false);
            return user.Object;
        }
    }
}
