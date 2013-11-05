using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// ViewLicenseInfo right
    /// </summary>
    public class ViewLicenseInfo : IRight
    {
        public static readonly Guid Id = new Guid("B6470F8E-C10A-4DB7-A2CA-87D15C6DC319");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "View license info"; } }
    }
}
