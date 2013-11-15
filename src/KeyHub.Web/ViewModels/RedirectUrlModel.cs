using KeyHub.Data;
using KeyHub.Web.ViewModels.User;
using System.Web;

namespace KeyHub.Web.ViewModels
{
    public class RedirectUrlModel
    {
        /// <summary>
        /// Parameterless Constructor is public so ViewModel can be a parameter in the Controller Method postback
        /// </summary>
        public RedirectUrlModel()
        { }

        public string RedirectUrl { get; set; }

        public void UseLocalReferrerAsRedirectUrl(HttpRequestBase request)
        {
            RedirectUrl = null;

            if (request.UrlReferrer == null)
                return;

            if (request.UrlReferrer.Host != request.Url.Host)
                return;

            RedirectUrl = request.UrlReferrer.ToString();
        }
    }
}