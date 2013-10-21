using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class FirstTest
    {
        [Fact]
        public void CanRunServer()
        {
            ResetLocalhostDatabase.Run();

            using (var site = new IISExpressDriver())
            {
                var path = Path.GetFullPath("../../../KeyHub.Web");

                if (!Directory.Exists(path))
                    throw new Exception("Could not find KeyHub.Web");

                site.Start("KeyHub.Web", "../../applicationhost.config");

                using (var client = new WebClient())
                {
                    Console.WriteLine(site.Url);
                    Console.WriteLine(client.DownloadString(site.Url));
                }

            }
        }
    }
}
