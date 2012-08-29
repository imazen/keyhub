using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// EditLicenseInfo right
    /// </summary>
    public class EditLicenseInfo : IRight
    {
        public static readonly Guid Id = new Guid("9F1ACAB8-0B3E-4533-ABF8-7F93793D65B6");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Edit license info"; } }
    }
}
