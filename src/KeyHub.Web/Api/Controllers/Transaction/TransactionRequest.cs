using System;

namespace KeyHub.Web.Api.Controllers.Transaction
{
    /// <summary>
    /// Transaction object
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
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