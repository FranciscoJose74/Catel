﻿namespace Catel.Logging
{
    using System;
    using Catel.Data;
    using Reflection;

    /// <summary>
    /// Extensions to the <see cref="ILog" /> interface.
    /// </summary>
    public static partial class LogExtensions
    {
        /// <summary>
        /// Exception data key used to indicates whether the exception was already processed by Catel log system.
        /// </summary>
        private const string AlreadyProcessedByCatelLogSystemExceptionDataKey = "AlreadyProcessedByCatelLogSystem";

        /// <summary>
        /// Logs the product info with version information.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void LogProductInfo(this ILog log)
        {
            ArgumentNullException.ThrowIfNull(log);

            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "PRODUCT INFO");
            Write(log, LogEvent.Info, string.Empty);

            var assembly = AssemblyHelper.GetEntryAssembly();
            if (assembly is not null)
            {
                Write(log, LogEvent.Info, "Assembly:              {0}", assembly.Title() ?? string.Empty);
                Write(log, LogEvent.Info, "Version:               {0}", assembly.Version());

                try
                {
                    Write(log, LogEvent.Info, "Informational version: {0}", assembly.InformationalVersion() ?? string.Empty);
                }
                catch (Exception)
                {
                    // Ignore
                }

                Write(log, LogEvent.Info, string.Empty);
                Write(log, LogEvent.Info, "Company:               {0}", assembly.Company() ?? string.Empty);
                Write(log, LogEvent.Info, "Copyright:             {0}", assembly.Copyright() ?? string.Empty);
            }

            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
        }

        /// <summary>
        /// Logs the device info.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void LogDeviceInfo(this ILog log)
        {
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "DEVICE INFO");
            Write(log, LogEvent.Info, string.Empty);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            Write(log, LogEvent.Info, "Platform:              {0}", Environment.OSVersion.Platform);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            Write(log, LogEvent.Info, "OS Version:            {0}", Environment.OSVersion.Version);

            Write(log, LogEvent.Info, "64-bit OS:             {0}", Environment.Is64BitOperatingSystem.ToString());
            Write(log, LogEvent.Info, "64-bit process:        {0}", Environment.Is64BitProcess.ToString());
            Write(log, LogEvent.Info, "Processor count:       {0}", Environment.ProcessorCount.ToString());
            Write(log, LogEvent.Info, "System page size:      {0}", Environment.SystemPageSize.ToString());

            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
        }

        /// <summary>
        /// Determines whether the log is Catel logging and can be ignored if Catel logging is disabled.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns><c>true</c> if the logging is Catel logging *and* can be ignored.</returns>
        public static bool IsCatelLoggingAndCanBeIgnored(this ILog log)
        {
            if (log is null)
            {
                return false;
            }

            if (!log.IsCatelLogging)
            {
                return false;
            }

            var catelLog = log as CatelLog;
            if (catelLog is not null && catelLog.AlwaysLog)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, object s1)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            log.WriteWithData(string.Format(messageFormat, s1), null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, object s1, object s2)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            log.WriteWithData(string.Format(messageFormat, s1, s2), null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, object s1, object s2, object s3)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            log.WriteWithData(string.Format(messageFormat, s1, s2, s3), null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, object s1, object s2, object s3, object s4)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

#pragma warning disable HAA0101 // Array allocation for params parameter
            log.WriteWithData(string.Format(messageFormat, s1, s2, s3, s4), null, logEvent);
#pragma warning restore HAA0101 // Array allocation for params parameter
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        /// <param name="s5">The formatting argument 5.</param>
        /// <param name="others">The formatting arguments.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (others is not null && others.Length > 0)
            {
                object[] args = { s1, s2, s3, s4, s5 };
                Array.Resize(ref args, 5 + others.Length);
                Array.Copy(others, 0, args, 5, others.Length);

                log.WriteWithData(string.Format(messageFormat, args), null, logEvent);
            }
            else
            {
#pragma warning disable HAA0101 // Array allocation for params parameter
                log.WriteWithData(string.Format(messageFormat, s1, s2, s3, s4, s5), null, logEvent);
#pragma warning restore HAA0101 // Array allocation for params parameter
            }
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            var message = messageFormat ?? string.Empty;
            if (args is not null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            log.WriteWithData(message, null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void Write(this ILog log, LogEvent logEvent, Exception exception, string messageFormat, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(log);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            var message = messageFormat ?? string.Empty;
            if (args is not null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            log.WriteWithData(exception, message, null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void WriteWithData(this ILog log, Exception exception, string message, object? extraData, LogEvent logEvent)
        {
            ArgumentNullException.ThrowIfNull(log);
            ArgumentNullException.ThrowIfNull(exception);

            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLoggingAndCanBeIgnored())
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreDuplicateExceptionLogging)
            {
                lock (exception)
                {
                    if (exception.Data.Contains(AlreadyProcessedByCatelLogSystemExceptionDataKey))
                    {
                        return;
                    }

                    exception.Data.Add(AlreadyProcessedByCatelLogSystemExceptionDataKey, BoxingCache.GetBoxedValue(true));
                }
            }

            log.WriteWithData(FormatException(exception, message), extraData, logEvent);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception ErrorAndCreateException<TException>(this ILog log, string messageFormat, object? arg1)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, (Exception?)null, string.Format(messageFormat, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <param name="arg2">Formatting argument 2.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception ErrorAndCreateException<TException>(this ILog log, string messageFormat, object? arg1, object? arg2)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, (Exception?)null, string.Format(messageFormat, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <param name="arg2">Formatting argument 2.</param>
        /// <param name="arg3">Formatting argument 3.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception ErrorAndCreateException<TException>(this ILog log, string messageFormat, object? arg1, object? arg2, object arg3)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, (Exception?)null, string.Format(messageFormat, arg1, arg2, arg3));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception ErrorAndCreateException<TException>(this ILog log, string messageFormat, params object[] args)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, (Exception?)null, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, null, createExceptionCallback, string.Format(messageFormat, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, null, createExceptionCallback, string.Format(messageFormat, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2, object? arg3)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, null, createExceptionCallback, string.Format(messageFormat, arg1, arg2, arg3));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, null, createExceptionCallback, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Exception innerException, string messageFormat, object? arg0)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, innerException, string.Format(messageFormat, arg0));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <param name="arg1">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Exception innerException, string messageFormat, object? arg0, object? arg1)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, innerException, string.Format(messageFormat, arg0, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <param name="arg1">The formatting argument.</param>
        /// <param name="arg2">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Exception innerException, string messageFormat, object? arg0, object? arg1, object? arg2)
            where TException : Exception
        {
            return ErrorAndCreateException<TException>(log, innerException, string.Format(messageFormat, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Exception? innerException, string messageFormat, params object?[] args)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(log);

            return ErrorAndCreateException<TException>(log, innerException, msg =>
            {
                var exception = ExceptionFactory.CreateException<TException>(msg, innerException);
                if (exception is null)
                {
                    var error = $"Exception type '{typeof(TException).Name}' does not have a constructor accepting a string";

                    log.Error(error);

                    throw new NotSupportedException(error);
                }

                return exception;
            }, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception ErrorAndCreateException<TException>(this ILog log, Exception? innerException, Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(log);

            var message = messageFormat ?? string.Empty;
            if (args is not null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            if (innerException is not null)
            {
                log.ErrorWithData(innerException, message);
            }
            else
            {
                log.ErrorWithData(message);
            }

            var exception = createExceptionCallback(message);
            if (exception is null)
            {
                var error = $"Exception type '{typeof(TException).Name}' does not have a constructor accepting a string";

                log.Error(error);

                throw new NotSupportedException(error);
            }

            return exception;
        }

        /// <summary>
        /// Formats the exception for logging with an additional message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <returns>Formatted string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        private static string FormatException(Exception exception, string message)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var formattedException = $"[{exception.GetType().Name}] {exception}";

            if (string.IsNullOrEmpty(message))
            {
                return formattedException;
            }

            return $"{message} | {formattedException}";
        }
    }
}
