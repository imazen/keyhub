using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Integration.Tests.TestSetup
{
    class KeyHubWebDriver : IISExpressDriver
    {
        public KeyHubWebDriver()
        {
            var path = Path.GetFullPath("../../../KeyHub.Web");

            if (!Directory.Exists(path))
                throw new Exception("Could not find KeyHub.Web");

            base.Start("KeyHub.Web", "../../applicationhost.config");
        }
    }
}
