using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class CleanDatabaseAttribute : BeforeAfterTestAttribute
    {
        private string GetDatabaseDirectory()
        {
            //return Path.GetFullPath("../../../KeyHub.Web/App_Data");
            return "c:\\temp";
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            using (var stopProcess = new PlainProcessDriver())
            {
                stopProcess.StartProcess("SQLLocalDB.exe", "stop v11.0");

                stopProcess.WaitForConsoleOutputMatching(@"(stopped\.)|(does not exist)");
            }

            using (var deleteProcess = new PlainProcessDriver())
            {
                deleteProcess.StartProcess("SQLLocalDB.exe", "delete v11.0");

                deleteProcess.WaitForConsoleOutputMatching(@"(deleted\.)|(does not exist)");
            }

            var dataDirectory = GetDatabaseDirectory();
            if (!Directory.Exists(dataDirectory))
                throw new Exception("Could not find data directory " + dataDirectory);

            foreach (var file in new[]
            {
                "KeyHub.mdf",
                "KeyHub_log.ldf"
            })
            {
                var fullFilepath = Path.Combine(dataDirectory, file);
                if (File.Exists(fullFilepath))
                    File.Delete(fullFilepath);
            }
        }
    }
}
