using System;

namespace KeyHub.Web.Api.Controllers
{
    class InvalidPropertyException : Exception
    {
        public InvalidPropertyException(string message)
            : base(message)
        {
        }
    }
}
