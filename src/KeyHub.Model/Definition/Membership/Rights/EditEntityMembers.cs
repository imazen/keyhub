using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// EditEntityMembers right
    /// </summary>
    public class EditEntityMembers : IRight
    {
        public static readonly Guid Id = new Guid("8F9EE04A-688C-46AA-819D-0EA70376B271");

        public Guid RightId { get { return Id; } }
        public string DisplayName { get { return "Edit entity members"; } }
    }
}
