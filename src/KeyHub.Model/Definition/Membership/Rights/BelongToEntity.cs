using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Model
{
    /// <summary>
    /// BelongToEntity right
    /// </summary>
    public class BelongToEntity : IRight
    {
        public static readonly Guid Id = new Guid("C967946E-EA10-47C4-BBEF-BAEBB3290FB2");

        public Guid RightId
        {
            get { return Id; }
        }

        public string DisplayName
        {
            get { return "Belong to entity"; }
        }
    }
}
