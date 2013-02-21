using System;
using System.Text;

namespace KeyHub.Core.Errors
{
    /// <summary>
    /// Represents a generic error
    /// </summary>
    public class GenericError : IError
    {
        /// <summary>
        /// Gets the exception associated with this error (if available)
        /// </summary>
        public Exception ErrorException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the message associated with this error
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the severity of this error
        /// </summary>
        public ErrorSeverity Severity
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the generated message from all fields in the error
        /// </summary>
        /// <returns></returns>
        public string GenerateMessage()
        {
            var builder = new StringBuilder();
            builder.Append("Generic error: " + ErrorMessage + ", Severity: " + Severity.ToString());

            if (ErrorException != null)
            {
                builder.AppendLine();

                Exception currentException = ErrorException;

                while (currentException != null)
                {
                    builder.AppendLine(currentException.Message);
                    builder.AppendLine(currentException.StackTrace);

                    currentException = currentException.InnerException;
                }
            }

            return builder.ToString();
        }

        public GenericError() { }

        public GenericError(Exception errorException, string errorMessage, ErrorSeverity severity)
        {
            ErrorException = errorException;
            ErrorMessage = errorMessage;
            Severity = severity;
        }
    }
}