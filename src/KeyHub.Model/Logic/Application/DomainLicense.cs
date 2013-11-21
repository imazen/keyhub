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
                return (AutomaticallyCreated && License.Sku.CanDeleteAutoDomains) ||
                       (!AutomaticallyCreated && License.Sku.CanDeleteManualDomains);
            }
        }
    }
}
