using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Microsoft.Web.WebPages.OAuth;

namespace KeyHub.Web
{
    public class AuthConfig
    {
        public static void RegisterAuth()
        {
            var microsoftClientId = WebConfigurationManager.AppSettings["microsoftClientId"];
            var microsoftClientSecret = WebConfigurationManager.AppSettings["microsoftClientSecret"];
            if (!string.IsNullOrEmpty(microsoftClientId) && !string.IsNullOrEmpty(microsoftClientSecret))
            {
                OAuthWebSecurity.RegisterMicrosoftClient(
                    clientId: microsoftClientId,
                    clientSecret: microsoftClientSecret);
            }

            var twitterConsumerKey = WebConfigurationManager.AppSettings["twitterConsumerKey"];
            var twitterConsumerSecret = WebConfigurationManager.AppSettings["twitterConsumerSecret"];
            if (!string.IsNullOrEmpty(twitterConsumerKey) && !string.IsNullOrEmpty(twitterConsumerSecret))
            {
                OAuthWebSecurity.RegisterTwitterClient(
                    consumerKey: twitterConsumerKey,
                    consumerSecret: twitterConsumerSecret);
            }

            var facebookAppId = WebConfigurationManager.AppSettings["facebookAppId"];
            var facebookAppSecret = WebConfigurationManager.AppSettings["facebookAppSecret"];
            if (!string.IsNullOrEmpty(facebookAppId) && !string.IsNullOrEmpty(facebookAppSecret))
            {
                OAuthWebSecurity.RegisterFacebookClient(
                    appId: facebookAppId,
                    appSecret: facebookAppSecret);
            }

            OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}