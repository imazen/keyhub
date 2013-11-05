using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Integration.Tests.TestSetup
{
    //  When using KeyHubWebDriver, it is more difficult to debug the server code since it
    //  runs in a separate process without the debugger.  To debug the server code in this case:
    //
    //  1.  Call Debugger.Launch() from the server-side code you want to debug (Debugger.Break does not work)
    //  2.  Run the test without the debugger
    //
    //  When the test hits the server-side code that calls Debugger.Launch(), you should be propmpted to
    //  choose a debugger for the process.

    public class KeyHubWebDriver : IISExpressDriver
    {
        public KeyHubWebDriver()
        {
            var path = Path.GetFullPath("../../../KeyHub.Web");

            if (!Directory.Exists(path))
                throw new Exception("Could not find KeyHub.Web");

            base.Start("KeyHub.Web");
        }

        public Uri UrlFor(string path)
        {
            return new Uri(new Uri(base.Url), path);
        }
    }
}
