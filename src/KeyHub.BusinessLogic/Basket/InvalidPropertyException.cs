using System;

namespace KeyHub.BusinessLogic.Basket
{
    class InvalidPropertyException : Exception
    {
        public InvalidPropertyException(string message)
            : base(message)
        {
        }
    }
}
