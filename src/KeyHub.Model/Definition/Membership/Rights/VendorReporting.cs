using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Vendor reporting right
    /// </summary>
    public class VendorReporting : IRight
    {
        public static readonly Guid Id = new Guid("7C733965-1F6B-45B2-861A-33A221C0723B");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Vendor reporting"; } }
    }
}
