using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using KeyHub;

namespace KeyHub.Common.Utils
{
    public static class CookieUtil
    {
        public static string GetCookieValue(HttpContext Context, string cookieName, bool decrypt)
        {
            if (!CookieExists(Context, cookieName))
                return string.Empty;

            string cookieVal = String.Empty;
            cookieVal = (decrypt) ? Context.Request.Cookies[cookieName].Value.Decrypt() : Context.Request.Cookies[cookieName].Value;
            return cookieVal;
        }

        public static void CreateCookie(HttpContext Context, string cookieName, string value, int? expirationDays, bool encrypt)
        {
            HttpCookie Cookie = null;
            Cookie = Context.Request.Cookies[cookieName];

            if (Cookie == null)
            {
                Cookie = new HttpCookie(cookieName, (encrypt) ? value.Encrypt() : value);
                Cookie.HttpOnly = false;
                Cookie.Domain = Context.Request.Url.DnsSafeHost;
                Cookie.Expires = DateTime.Now.AddDays(expirationDays.Value);
                Context.Response.Cookies.Add(Cookie);
            }
            else
            {
                Cookie.HttpOnly = false;
                Cookie.Domain = Context.Request.Url.DnsSafeHost;
                Cookie.Expires = DateTime.Now.AddDays(expirationDays.Value);
                Cookie.Value = (encrypt) ? value.Encrypt() : value;
                Context.Response.Cookies.Add(Cookie);
            }
        }

        public static void DeleteCookie(HttpContext Context, string cookieName)
        {
            if (Context.Request.Cookies[cookieName] != null)
            {
                HttpCookie myCookie = new HttpCookie(cookieName);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Context.Response.Cookies.Add(myCookie);
            }
        }

        public static bool CookieExists(HttpContext Context, string cookieName)
        {
            bool exists = false;
            HttpCookie cookie = Context.Request.Cookies[cookieName];
            if (cookie != null)
                exists = true;
            return exists;
        }

        public static Dictionary<string, string> GetAllCookies(HttpContext Context, bool decrypt)
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();
            foreach (string key in Context.Request.Cookies.AllKeys)
            {
                cookies.Add(key, (decrypt) ? Context.Request.Cookies[key].Value.Decrypt() : Context.Request.Cookies[key].Value);
            }
            return cookies;
        }

        public static void DeleteAllCookies(HttpContext Context)
        {
            var x = Context.Request.Cookies;
            foreach (HttpCookie cook in x)
            {
                DeleteCookie(Context, cook.Name);
            }
        }
    }
}
