using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Vendor administrator right
    /// </summary>
    public class VendorAdmin : IRight
    {
        public static readonly Guid Id = new Guid("5b915b1e-95bb-4e90-bbd4-79bbb378d555");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Vendor admin"; } }
    }
}
