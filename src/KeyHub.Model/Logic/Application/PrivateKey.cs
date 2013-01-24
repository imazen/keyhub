using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    public partial class PrivateKey
    {
        /// <summary>
        /// Generates keybytes for the private key
        /// </summary>
        public void SetKeyBytes()
        {
            //TODO: Who provides the Key?
            this.KeyBytes = new RSACryptoServiceProvider(2048).ExportCspBlob(true);
        }

        public string GetPrivateKeyXmlString()
        {
            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.ImportCspBlob(KeyBytes);
                return r.ToXmlString(true);
            }
        }

        public void SetPrivateKeyXmlString(string privateKeyXmlString)
        {
            using (var r = new RSACryptoServiceProvider(2048))
            {
                r.FromXmlString(privateKeyXmlString);
                KeyBytes = r.ExportCspBlob(true);
            }
        }
    }
}
