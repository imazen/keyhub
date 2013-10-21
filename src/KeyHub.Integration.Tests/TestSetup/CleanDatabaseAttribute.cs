using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class CleanDatabaseAttribute : BeforeAfterTestAttribute
    {
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
        }
    }
}
