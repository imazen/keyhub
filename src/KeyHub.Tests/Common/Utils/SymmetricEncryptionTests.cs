using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Common.Utils;
using Xunit;

namespace KeyHub.Tests.Common.Utils
{
    public class SymmetricEncryptionTests
    {
        [Fact]
        public void CanEncryptDecrypt()
        {
            var privateKey = "this is my encryption key!!";
            byte[] original = Encoding.UTF8.GetBytes("Hello world");

            var encrypted = SymmetricEncryption.Encrypt(original, privateKey);
            var decrypted = SymmetricEncryption.Decrypt(encrypted, privateKey);

            Assert.Equal(original, decrypted);
        }
    }
}
