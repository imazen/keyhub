using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class PlainProcessDriver : ProcessDriver
    {
        public void StartProcess(string exePath, string arguments = "")
        {
            base.StartProcess(exePath, arguments);
        }

        public Match WaitForConsoleOutputMatching(string pattern, int msMaxWait = 10000, int msWaitInterval = 500)
        {
            return base.WaitForConsoleOutputMatching(pattern, msMaxWait, msWaitInterval);
        }
    }
}
