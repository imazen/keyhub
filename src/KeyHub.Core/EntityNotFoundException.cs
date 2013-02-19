using System;

namespace KeyHub.Core
{
    /// <summary>
    /// Exception thrown when requested entity does not exist in the current datacontext
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }
}
