using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeyHub.Common.Utils
{
    public class UrlService
    {
        public static string Base64UrlEncode(string input)
        {
            var MyBytes = Encoding.UTF8.GetBytes(input);
            return HttpServerUtility.UrlTokenEncode(MyBytes);
        }

        public static string Base64UrlDecode(string input)
        {
            var MyBytes = HttpServerUtility.UrlTokenDecode(input);
            return Encoding.UTF8.GetString(MyBytes);
        }
    }
}