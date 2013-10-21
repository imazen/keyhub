using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyHub.Integration.Tests.TestSetup;
using Xunit;

namespace KeyHub.Integration.Tests
{
    public class FirstTest
    {
        [Fact]
        [CleanDatabaseAttribute]
        public void CanRunServer()
        {
            using (var site = new KeyHubWebDriver())
            {
                using (var client = new WebClient())
                {
                    Console.WriteLine(site.Url);
                    Console.WriteLine(client.DownloadString(site.Url));
                }
            }
        }
    }
}
