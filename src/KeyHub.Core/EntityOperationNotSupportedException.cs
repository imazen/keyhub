using System;

namespace KeyHub.Core
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
