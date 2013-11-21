using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.User
{
    public class LinkAccountModel
    {
        public IEnumerable<string> OpenIDProvidersAvailable;
        public IEnumerable<string> OpenIDProvidersLinked;
        public bool AllowRemovingLogin;
    }
}