using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using KeyHub.Data;
using Microsoft.Web.WebPages.OAuth;

namespace KeyHub.Web.ViewModels.User
{
    public class LinkAccountModel
    {
        public IEnumerable<string> OpenIDProvidersAvailable;
        public IEnumerable<string> OpenIDProvidersLinked;
        public bool AllowRemovingLogin;

        public static LinkAccountModel ForUser(IDataContext context, IIdentity identity)
        {
            var user = context.GetUser(identity);

            var allProviders = OAuthWebSecurity.RegisteredClientData.Select(c => c.DisplayName).ToArray();

            //  Match each linked provider to the member of allProviders as allProviders has proper casing (Google, not google)
            var linkedProviders = OAuthWebSecurity.GetAccountsFromUserName(user.MembershipUserIdentifier)
                .Select(lp => allProviders.Single(ap => ap.ToLower() == lp.Provider.ToLower()))
                .ToArray();

            var loginMethodCount = linkedProviders.Count() + (OAuthWebSecurity.HasLocalAccount(user.UserId) ? 1 : 0);

            var model = new LinkAccountModel()
            {
                OpenIDProvidersLinked = linkedProviders,
                OpenIDProvidersAvailable = allProviders.Where(p => !linkedProviders.Contains(p)),
                AllowRemovingLogin = loginMethodCount > 1
            };
            return model;
        }
    }
}