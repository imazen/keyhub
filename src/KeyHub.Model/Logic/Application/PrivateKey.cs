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
            //Generate a new private key
            using(var r = new RSACryptoServiceProvider(2048, new CspParameters() { Flags = CspProviderFlags.CreateEphemeralKey | CspProviderFlags.NoPrompt})){
                try{
                    this.KeyBytes = r.ExportCspBlob(true);
                }finally{
                    r.PersistKeyInCsp = false;
                }
            }
        }

        public string GetPublicKeyXmlString()
        {
            return GetXmlString(false);
        }

        public string GetPrivateKeyXmlString()
        {
            return GetXmlString(true);
        }

        public string GetXmlString(bool includePrivate)
        {
            using (var r = new RSACryptoServiceProvider(2048, new CspParameters() { Flags = CspProviderFlags.CreateEphemeralKey | CspProviderFlags.NoPrompt }))
            {
                try
                {
                    r.ImportCspBlob(KeyBytes);
                    return r.ToXmlString(includePrivate);
                }
                finally
                {
                    r.PersistKeyInCsp = false; //Default behavior is to store on filesystem; this is a security issue
                }
            }

        }

        public void SetPrivateKeyXmlString(string privateKeyXmlString)
        {
            using (var r = new RSACryptoServiceProvider(2048, new CspParameters() { Flags = CspProviderFlags.CreateEphemeralKey | CspProviderFlags.NoPrompt }))
            {
                try{
                    r.FromXmlString(privateKeyXmlString);
                    KeyBytes = r.ExportCspBlob(true);
                }
                finally
                {
                    r.PersistKeyInCsp = false; //Default behavior is to store on filesystem; this is a security issue
                }
            }
        }
    }
}
