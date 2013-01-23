using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Tests.TestData
{
    public static class PrivateKeyTestData
    {
        public static PrivateKey Create()
        {
            return new PrivateKey
            {
                PrivateKeyId = Guid.NewGuid(),
                KeyBytes = new RSACryptoServiceProvider(2048).ExportCspBlob(true)
            };
        }
    }
}
