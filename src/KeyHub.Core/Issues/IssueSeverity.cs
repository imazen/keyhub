namespace KeyHub.Core.Issues
{
    /// <summary>
    /// Defines all issue severities supported
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>
        /// Severe error which causes the application to crash.
        /// </summary>
        Critical,

        /// <summary>
        /// Errors which causes the user to discontinue work
        /// </summary>
        Error,

        /// <summary>
        /// Non-breaking errors (warnings) to let the developers know something might be wrong
        /// </summary>
        Warning
    }
}