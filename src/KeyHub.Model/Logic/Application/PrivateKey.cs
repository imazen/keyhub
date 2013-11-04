using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Common.Utils;

namespace KeyHub.Model
{
    public partial class PrivateKey
    {
        /// <summary>
        /// Generates keybytes for the private key
        /// </summary>
        public void SetKeyBytes()
        {
            //Generate a new private key
            using(var r = new RSACryptoServiceProvider(2048, new CspParameters() { Flags = CspProviderFlags.CreateEphemeralKey | CspProviderFlags.NoPrompt})){
                try
                {
                    var privateKeyBytes = r.ExportCspBlob(true);
                    this.KeyBytes = SymmetricEncryption.Encrypt(privateKeyBytes, ConfigurationManager.AppSettings["DatabaseEncryptionKey"]);
                }
                finally{
                    r.PersistKeyInCsp = false;
                }
            }
        }

        public string GetPublicKeyXmlString()
        {
            using (var r = new RSACryptoServiceProvider(2048, new CspParameters()
            {
                Flags = CspProviderFlags.CreateEphemeralKey | CspProviderFlags.NoPrompt
            }))
            {
                try
                {
                    var privateKey = SymmetricEncryption.Decrypt(KeyBytes, ConfigurationManager.AppSettings["DatabaseEncryptionKey"]);

                    r.ImportCspBlob(privateKey);
                    return r.ToXmlString(false);
                }
                finally
                {
                    r.PersistKeyInCsp = false; //Default behavior is to store on filesystem; this is a security issue
                }
            }
        }
    }
}
