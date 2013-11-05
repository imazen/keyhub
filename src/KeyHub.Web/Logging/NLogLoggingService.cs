using System;
using KeyHub.Core.Errors;
using KeyHub.Core.Logging;

namespace KeyHub.Web.Logging
{
    public sealed class NLogLoggingService : ILoggingService
    {
        private NLog.Logger logger;

        private NLog.LogLevel ConvertLogLevel(LogTypes logType)
        {
            switch (logType)
            {
                case LogTypes.Debug:
                    return NLog.LogLevel.Debug;
                case LogTypes.Error:
                    return NLog.LogLevel.Error;
                case LogTypes.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogTypes.Info:
                    return NLog.LogLevel.Info;
                case LogTypes.Trace:
                    return NLog.LogLevel.Trace;
                case LogTypes.Warn:
                    return NLog.LogLevel.Warn;
                default:
                    return NLog.LogLevel.Off;
            }
        }

        public NLogLoggingService()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void Log(string message)
        {
            logger.Log(NLog.LogLevel.Info, message);
        }

        public void Log(string message, LogTypes type)
        {
            logger.Log(ConvertLogLevel(type), message);
        }

        public void Log(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Info, x.GenerateMessage()));
        }

        public void Log(LogTypes type, params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(ConvertLogLevel(type), x.GenerateMessage()));
        }

        public void Info(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Info, x.GenerateMessage()));
        }

        public void Info(string message, params object[] args)
        {
            logger.Log(NLog.LogLevel.Info, message, args);
        }

        public void Debug(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Debug, x.GenerateMessage()));
        }

        public void Debug(string message, params object[] args)
        {
            logger.Log(NLog.LogLevel.Debug, message, args);
        }

        public void Warn(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Warn, x.GenerateMessage()));
        }

        public void Warn(string message, params object[] args)
        {
            logger.Log(NLog.LogLevel.Warn, message, args);
        }

        public void Error(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Error, x.GenerateMessage()));
        }

        public void Error(string message, params object[] args)
        {
            logger.Log(NLog.LogLevel.Error, message, args);
        }

        public void Fatal(params IError[] errors)
        {
            Array.ForEach<IError>(errors, x => logger.Log(NLog.LogLevel.Fatal, x.GenerateMessage()));
        }

        public void Fatal(string message, params object[] args)
        {
            logger.Log(NLog.LogLevel.Fatal, message, args);
        }

        public void Dispose()
        {
            logger.Factory.Dispose();
            logger = null;

            GC.SuppressFinalize(this);
        }
    }
}