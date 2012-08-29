using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// EditEntityInfo right
    /// </summary>
    public class EditEntityInfo : IRight
    {
        public static readonly Guid Id = new Guid("BA67F752-6357-4411-947D-40036AA07C31");
        
        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Belong to entity"; } }
    }
}
