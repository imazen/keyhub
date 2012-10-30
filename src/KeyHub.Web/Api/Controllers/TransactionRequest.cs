using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// Transaction object
    /// </summary>
    public class TransactionRequest
    {
        public Guid[] PurchasedSkus { get; set; }
        public string PurchaserName { get; set; }
        public string PurchaserEmail { get; set; }
    }
}