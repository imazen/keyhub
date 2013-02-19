using System;
using KeyHub.Core.Logging;

namespace KeyHub.Runtime
{
    ///// <summary>
    ///// Provides runtime access to the current logger implementation
    ///// </summary>
    //public sealed class LogContext : IDisposable
    //{
    //    /// <summary>
    //    /// Holds the current ILogger implementation
    //    /// </summary>
    //    public static volatile ILogger Instance;

    //    /// <summary>
    //    /// Disposes the current ILogger implementation
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        // Dispose the logger if we have an instance
    //        if (Instance != null)
    //            Instance.Dispose();
    //    }

    //    static LogContext()
    //    {
    //        Instance = DependencyContext.Instance.GetExportedValue<ILogger>();
    //    }
    //}
}