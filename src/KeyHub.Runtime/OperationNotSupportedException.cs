using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Runtime
{
    /// <summary>
    /// Exception thrown when requested operation on entity is not supported
    /// </summary>
    public class EntityOperationNotSupportedException : Exception
    {
        public EntityOperationNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
