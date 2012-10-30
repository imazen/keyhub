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
        [System.Xml.Serialization.XmlElementAttribute("PurchasedSku")]
        public Guid[] PurchasedSkus { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PurchaserName { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PurchaserEmail { get; set; }
    }
}