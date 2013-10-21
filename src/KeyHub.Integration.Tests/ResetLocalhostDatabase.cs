using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Integration.Tests
{
    public class ResetLocalhostDatabase : ProcessDriver
    {
        public static void Run()
        {
            using (var stopProcess = new ResetLocalhostDatabase())
            {
                stopProcess.StartProcess("SQLLocalDB.exe", "stop v11.0");

                stopProcess.WaitForConsoleOutputMatching(@"(stopped\.)|(does not exist)");
            }

            using (var deleteProcess = new ResetLocalhostDatabase())
            {
                deleteProcess.StartProcess("SQLLocalDB.exe", "delete v11.0");

                deleteProcess.WaitForConsoleOutputMatching(@"(deleted\.)|(does not exist)");
            }
        }
    }
}
