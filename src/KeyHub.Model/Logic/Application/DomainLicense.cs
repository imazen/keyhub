using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    public partial class DomainLicense
    {
        /// <summary>
        /// Generates keybytes for the private key
        /// </summary>
        public void SetKeyBytes()
        {

        }

        public bool IsNew
        {
            get { return DomainLicenseId == default(Guid); }
        }

        public bool CanBeManuallyDeleted
        {
            get
            {
                return (AutomaticlyCreated && License.Sku.CanDeleteAutoDomains) ||
                       (!AutomaticlyCreated && License.Sku.CanDeleteManualDomains);
            }
        }
    }
}
