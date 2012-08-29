using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// GrantMemberRights right
    /// </summary>
    public class GrantMemberRights : IRight
    {
        public static readonly Guid Id = new Guid("E39D63AF-B5F7-4057-9D0E-46DB2B20161C");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Grant member rights"; } }
    }
}
