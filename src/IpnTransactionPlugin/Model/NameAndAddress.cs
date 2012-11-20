using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpnTransactionPlugin.Extensions;

namespace IpnTransactionPlugin.Model
{
    /// <summary>
    /// Contains name and address information; used for Transaction.Billing and Transaction.Shipping
    /// </summary>
    public class NameAndAddress
    {
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        /// <summary>
        /// Parse a NameAndAddress instance from a provided NameValueCollection
        /// </summary>
        /// <param name="data">NameValue data collection to pluck the address from</param>
        /// <param name="prefix">Data field prefix</param>
        /// <returns>NameAndAddress instance</returns>
        public static NameAndAddress ParseFrom(NameValueCollection data, string prefix)
        {
            return new NameAndAddress
            {
                BusinessName = data.Pluck(prefix + "business_name"),
                Name = data.Pluck(prefix + "name"),
                Street = data.Pluck(prefix + "street"),
                Phone = data.Pluck(prefix + "phone"),
                City = data.Pluck(prefix + "city"),
                Zip = data.Pluck(prefix + "zip"),
                State = data.Pluck(prefix + "state"),
                Country = data.Pluck(prefix + "country"),
                CountryCode = data.Pluck(prefix + "country_code")
            };
        }
    }
}
