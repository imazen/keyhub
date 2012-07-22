using System;

namespace KeyHub.Core.Issues
{
    /// <summary>
    /// Represents a generic issue that can be picked up by the current logging class
    /// </summary>
    public interface IIssue
    {
        /// <summary>
        /// Gets the severity of this issue
        /// </summary>
        IssueSeverity Severity { get; set; }

        /// <summary>
        /// Gets the exception associated with this issue (if available)
        /// </summary>
        Exception IssueException { get; set; }

        /// <summary>
        /// Gets the message associated with this issue
        /// </summary>
        string IssueMessage { get; set; }

        /// <summary>
        /// Returns the generated message from all fields in the issue
        /// </summary>
        /// <returns></returns>
        string GenerateMessage();
    }
}