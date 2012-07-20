using System;
using KeyHub.Core.Issues;

namespace KeyHub.Core.Logging
{
    /// <summary>
    /// Defines the logging options for KeyHub
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Writes the diagnostic message for the default level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        void Log(string message);

        /// <summary>
        /// Writes the diagnostic message for the specified level
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="type">The log type.</param>
        void Log(string message, LogTypes type);

        /// <summary>
        /// Writes the issue for the default level
        /// </summary>
        /// <param name="issues">The issue to be written</param>
        void Log(params IIssue[] issues);

        /// <summary>
        /// Writes the issue for the specified level
        /// </summary>
        /// <param name="issues">The issue to be written</param>
        /// <param name="type">The log type</param>
        void Log(LogTypes type, params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="issues">The issues to be written</param>
        void Info(params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Additional formatting parameters</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="issues">The issues to be written</param>
        void Debug(params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Additional formatting parameters</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Writes the diagnostic message at the Warning level.
        /// </summary>
        /// <param name="issues">The issues to be written</param>
        void Warn(params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Warning level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Additional formatting parameters</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="issues">The issues to be written</param>
        void Error(params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Additional formatting parameters</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="issues">The issues to be written</param>
        void Fatal(params IIssue[] issues);

        /// <summary>
        /// Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Additional formatting parameters</param>
        void Fatal(string message, params object[] args);
    }
}