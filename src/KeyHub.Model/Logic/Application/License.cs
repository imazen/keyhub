using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    public partial class License
    {
        /// <summary>
        /// Constructor that creates a license based on a provided SKU 
        /// </summary>
        /// <param name="licensedSku">SKU this license is based on</param>
        /// <param name="owner">Owning customer</param>
        /// <param name="purchaser">Purchasing customer</param>
        public License(SKU licensedSku, Customer owner, Customer purchaser)
        {
            this.SkuId = licensedSku.SkuId;
            this.LicenseIssued = DateTime.Now;
            this.PurchasingCustomerId = purchaser.ObjectId;
            this.OwningCustomerId = owner.ObjectId;
            this.OwnerName = owner.Name;

            if (licensedSku.LicenseDuration.HasValue)
            {
                this.LicenseExpires = DateTime.Now.AddMonths(licensedSku.LicenseDuration.Value);
            }
        }
    }
}
