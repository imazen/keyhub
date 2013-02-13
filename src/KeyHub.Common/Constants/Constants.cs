using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Common
{
    public static class Constants
    {
        public static readonly string ConnectionStringName = "DataContext";
        public static readonly string DefaultPasswordEncryption = "KeyHubAbcd!@#";
        public static readonly string BasketCookieName = "KeyHub_BaseketCookie";
        public static readonly int BasketCookieExpirationDays = 1;
    }
}
