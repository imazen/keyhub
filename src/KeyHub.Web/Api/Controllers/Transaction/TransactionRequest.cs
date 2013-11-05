using System.Xml.Serialization;

namespace KeyHub.Web.Api.Controllers.Transaction
{
    /// <summary>
    /// Transaction object
    /// </summary>
    [XmlTypeAttribute(AnonymousType = true), XmlRoot(Namespace = "", IsNullable = false)]
    public class TransactionRequest
    {
        [XmlElementAttribute("PurchasedSku")]
        public string[] PurchasedSkus { get; set; }

        [XmlAttributeAttribute]
        public string PurchaserName { get; set; }

        [XmlAttributeAttribute]
        public string PurchaserEmail { get; set; }
    }
}