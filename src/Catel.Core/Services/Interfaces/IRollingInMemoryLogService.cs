﻿namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// Rolling in memory log service.
    /// </summary>
    public interface IRollingInMemoryLogService
    {
        /// <summary>
        /// Gets or sets the maximum number of log entries to keep.
        /// <para />
        /// The default value is 250.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        int MaximumNumberOfLogEntries { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of warning log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        int MaximumNumberOfWarningLogEntries { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of error log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        int MaximumNumberOfErrorLogEntries { get; set; }

        /// <summary>
        /// Gets the log listener.
        /// </summary>
        /// <value>The log listener.</value>
        RollingInMemoryLogListener LogListener { get; }

        /// <summary>
        /// Occurs when a log message is written.
        /// </summary>
        event EventHandler<LogMessageEventArgs>? LogMessage;

        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        IEnumerable<LogEntry> GetLogEntries();

        /// <summary>
        /// Gets the warning log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        IEnumerable<LogEntry> GetWarningLogEntries();

        /// <summary>
        /// Gets the error log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        IEnumerable<LogEntry> GetErrorLogEntries();
    }
}
