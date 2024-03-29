﻿// Define debug so this code doesn't get stripped out
#define DEBUG

namespace Catel.Logging
{
    using System;

    public partial class DebugLogListener
    {
        /// <summary>
        /// Writes the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object? extraData, LogData? logData, DateTime time)
        {
            var consoleMessage = FormatLogEvent(log, message, logEvent, extraData, logData, time);

            switch (logEvent)
            {
                case LogEvent.Debug:
                    System.Diagnostics.Trace.WriteLine(consoleMessage);
                    break;

                case LogEvent.Info:
                    System.Diagnostics.Trace.TraceInformation(consoleMessage);
                    break;

                case LogEvent.Warning:
                    System.Diagnostics.Trace.TraceWarning(consoleMessage);
                    break;

                case LogEvent.Error:
                    System.Diagnostics.Trace.TraceError(consoleMessage);
                    break;
            }
        }
    }
}
