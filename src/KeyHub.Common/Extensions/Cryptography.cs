﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyHub
{
    public static class Cryptography
    {
        public static string GetMD5HashCode(this string str)
        {
            return KeyHub.Common.Utils.Cryptography.Hashing.Hash(str);
        }

        public static string Encrypt(this string InputString)
        {
            return KeyHub.Common.Utils.Cryptography.Encryption.EncryptText(InputString, KeyHub.Common.Utils.Cryptography.Encryption.EncryptionTypes.Rijndael);
        }

        public static string Decrypt(this string InputString)
        {
            return KeyHub.Common.Utils.Cryptography.Encryption.DecryptText(InputString, KeyHub.Common.Utils.Cryptography.Encryption.EncryptionTypes.Rijndael);
        }
    }
}