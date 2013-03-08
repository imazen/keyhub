using System;

namespace KeyHub.Data
{
    public class DbSetNotReadyException : Exception
    {
        public DbSetNotReadyException(string message)
            : base(message)
        {
        }
    }
}