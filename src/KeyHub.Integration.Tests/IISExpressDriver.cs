using System;
using System.Diagnostics;
using System.IO;

namespace KeyHub.Integration.Tests
{
    class IISExpressDriver : ProcessDriver
    {
        public string Url { get; private set;  }

        public void Start(string siteName, string configPath)
        {
            var processFileName = File.Exists(@"c:\program files (x86)\IIS Express\IISExpress.exe")
                ? @"c:\program files (x86)\IIS Express\IISExpress.exe"
                : @"c:\program files\IIS Express\IISExpress.exe";

            var fullConfigPath = Path.GetFullPath(configPath);
            if (!File.Exists(fullConfigPath))
                throw new Exception("Could not find config file " + configPath);

            StartProcess(processFileName, "/systray:false /trace:error /site:\"" + siteName + "\" /config:\"" + fullConfigPath + "\"");

            var match = WaitForConsoleOutputMatching(@"Successfully registered URL ""([^""]*)""");

            Url = match.Groups[1].Value;
        }

        protected override void Shutdown()
        {
            try
            {
                _process.Kill();
            }
            catch (Exception)
            {
            }

            if (!_process.WaitForExit(10000))
                throw new Exception("IISExpress did not halt within 10 seconds.");
        }
    }
}
