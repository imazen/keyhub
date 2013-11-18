using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    public class PrivateKeyRemoveModel : RedirectUrlModel
    {
        public Guid PrivateKeyId { get; set; }
        public string PrivateKeyName { get; set; }
        public string VendorName { get; set; }
    }
}