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
