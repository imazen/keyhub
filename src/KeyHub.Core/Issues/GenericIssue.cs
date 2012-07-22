using System;
using System.Text;

namespace KeyHub.Core.Issues
{
    /// <summary>
    /// Represents a generic issue
    /// </summary>
    public class GenericIssue : IIssue
    {
        /// <summary>
        /// Gets the exception associated with this issue (if available)
        /// </summary>
        public Exception IssueException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the message associated with this issue
        /// </summary>
        public string IssueMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the severity of this issue
        /// </summary>
        public IssueSeverity Severity
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the generated message from all fields in the issue
        /// </summary>
        /// <returns></returns>
        public string GenerateMessage()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Generic issue: " + IssueMessage + ", Severity: " + Severity.ToString());

            if (IssueException != null)
            {
                builder.AppendLine();

                Exception currentException = IssueException;

                while (currentException != null)
                {
                    builder.AppendLine(currentException.Message);
                    builder.AppendLine(currentException.StackTrace);

                    currentException = currentException.InnerException;
                }
            }

            return builder.ToString();
        }

        public GenericIssue() { }

        public GenericIssue(Exception issueException, string issueMessage, IssueSeverity severity)
        {
            IssueException = issueException;
            IssueMessage = issueMessage;
            Severity = severity;
        }
    }
}