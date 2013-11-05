using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Common.Utils
{
    /// <summary>
    /// SymmetricEncryption will use AesCryptoServiceProvider to encrypt/decrypt data with
    /// a private string.  A salt (initialization vector) is generated automatically and
    /// included with the encrypted result.
    /// 
    /// Based off of code from:
    /// http://stackoverflow.com/questions/8041451/good-aes-initialization-vector-practice
    /// </summary>
    public class SymmetricEncryption
    {
        public static byte[] Encrypt(byte[] data, string privateKey)
        {
            //  We hash the private key string to ensure we have a key with the correct
            //  number of bytes.
            var privateKeyHash = HashKey(privateKey);

            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = privateKeyHash;
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    ms.Write(provider.IV, 0, 16);
                    using (var encryptor = provider.CreateEncryptor())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] Decrypt(byte[] encryptedString, string privateKey)
        {
            //  We hash the private key string to ensure we have a key with the correct
            //  number of bytes.
            var privateKeyHash = HashKey(privateKey);

            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = privateKeyHash;
                using (var ms = new MemoryStream(encryptedString))
                {
                    // Read the first 16 bytes which is the IV.
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, 16);
                    provider.IV = iv;

                    using (var decryptor = provider.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            return ReadToEnd(cs);
                        }
                    }
                }
            }
        }

        private static byte[] HashKey(string privateKey)
        {
            byte[] privateKeyHash;
            using (var sha256 = new SHA256CryptoServiceProvider())
            {
                privateKeyHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(privateKey));
            }
            return privateKeyHash;
        }

        public static byte[] ReadToEnd(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
