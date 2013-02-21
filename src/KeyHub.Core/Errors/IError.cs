using System;

namespace KeyHub.Core.Errors
{
    /// <summary>
    /// Represents a generic error that can be picked up by the current logging class
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Gets the severity of this error
        /// </summary>
        ErrorSeverity Severity { get; set; }

        /// <summary>
        /// Gets the exception associated with this error (if available)
        /// </summary>
        Exception ErrorException { get; set; }

        /// <summary>
        /// Gets the message associated with this error
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Returns the generated message from all fields in the error
        /// </summary>
        /// <returns></returns>
        string GenerateMessage();
    }
}