﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Logging
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Reflection;
    using System.Threading.Tasks;

    using Catel.IO;

    using Path = System.IO.Path;

    /// <summary>
    /// Log listener which writes all data to a file.
    /// </summary>
    public class FileLogListener : BatchLogListenerBase
    {
        /// <summary>
        /// Defines the keywords that can be used in the <see cref="FilePath"/> property to inject different values.
        /// </summary>
        public static class FilePathKeyword
        {
            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to create a log with the following format: 
            /// <para/>
            /// <c>{AssemblyName}_{Date}_{Time}_{ProcessId}</c>
            /// </summary>
            public const string AutoLogFileName = "{AutoLogFileName}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert <c>%ProgramData%</c> or <c>%AppData%</c> into the filepath.
            /// <para/>
            /// If the application is a web app (System.Web is referenced); then <c>%ProgramData%</c> is used; otherwise <c>%AppData%</c> is used.
            /// </summary>
            public const string AppData = "{AppData}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert <c>%LocalAppData%</c> into the filepath.
            /// </summary>
            public const string AppDataLocal = "{AppDataLocal}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert <c>%AppData%</c> into the filepath.
            /// </summary>
            public const string AppDataRoaming = "{AppDataRoaming}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert <c>%ProgramData%</c> into the filepath.
            /// </summary>
            public const string AppDataMachine = "{AppDataMachine}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>AppDomain.CurrentDomain.BaseDirectory</c> into the filepath.
            /// </summary>
            public const string AppDir = "{AppDir}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>Assembly.Company()</c> value.
            /// </summary>
            public const string AssemblyCompany = "{AssemblyCompany}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>Assembly.GetName().Name</c> value.
            /// </summary>
            public const string AssemblyName = "{AssemblyName}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>Assembly.Product()</c> value.
            /// </summary>
            public const string AssemblyProduct = "{AssemblyProduct}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the date as <c>yyyy-MM-dd</c>.
            /// </summary>
            public const string Date = "{Date}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the time as <c>HHmmss</c>.
            /// </summary>
            public const string Time = "{Time}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>Process.GetCurrentProcess().Id</c> value.
            /// </summary>
            public const string ProcessId = "{ProcessId}";

            /// <summary>
            /// Keyword that can be used within the <see cref="FilePath"/> to insert the <c>Directory.GetCurrentDirectory()</c> value.
            /// </summary>
            public const string WorkDir = "{WorkDir}";
        }

        private readonly string AutoLogFileNameReplacement = string.Format("{0}_{1}_{2}_{3}", FilePathKeyword.AssemblyName, FilePathKeyword.Date, FilePathKeyword.Time, FilePathKeyword.ProcessId);

        private Assembly _assembly;
        private string _filePath;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        public FileLogListener(Assembly assembly = null)
        {
            Initialize(true, assembly);

            MaxSizeInKiloBytes = 1000 * 10; // 10 MB
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxSizeInKiloBytes">The max size in kilo bytes.</param>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        /// <exception cref="ArgumentException">The <paramref name="filePath" /> is <c>null</c> or whitespace.</exception>
        public FileLogListener(string filePath, int maxSizeInKiloBytes, Assembly assembly = null)
        {
            Argument.IsNotNullOrWhitespace(() => filePath);

            Initialize(false, assembly);

            FilePath = filePath;
            MaxSizeInKiloBytes = maxSizeInKiloBytes;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = DetermineFilePath(value); }
        }

        /// <summary>
        /// Gets or sets the maximum size information kilo bytes.
        /// </summary>
        /// <value>The maximum size information kilo bytes.</value>
        public int MaxSizeInKiloBytes { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines the real file path.
        /// </summary>
        /// <param name="filePath">The file path to examine.</param>
        /// <returns>The real file path.</returns>
        protected virtual string DetermineFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Empty;
            }

            if (filePath.Contains(FilePathKeyword.AutoLogFileName))
            {
                filePath = filePath.Replace(FilePathKeyword.AutoLogFileName, AutoLogFileNameReplacement);
            }

            var isWebApp = HttpContextHelper.HasHttpContext();

            string dataDirectory;

            if (_assembly != null)
            {
                dataDirectory = isWebApp ? IO.Path.GetApplicationDataDirectoryForAllUsers(_assembly.Company(), _assembly.Product())
                                         : IO.Path.GetApplicationDataDirectory(_assembly.Company(), _assembly.Product());
            }
            else
            {
                dataDirectory = isWebApp ? IO.Path.GetApplicationDataDirectoryForAllUsers()
                                         : IO.Path.GetApplicationDataDirectory();
            }

            if (filePath.Contains(FilePathKeyword.AssemblyName))
            {
                filePath = filePath.Replace(FilePathKeyword.AssemblyName, _assembly.GetName().Name);
            }

            if (filePath.Contains(FilePathKeyword.AssemblyProduct))
            {
                filePath = filePath.Replace(FilePathKeyword.AssemblyProduct, _assembly.Product());
            }

            if (filePath.Contains(FilePathKeyword.AssemblyCompany))
            {
                filePath = filePath.Replace(FilePathKeyword.AssemblyCompany, _assembly.Company());
            }

            if (filePath.Contains(FilePathKeyword.ProcessId))
            {
                filePath = filePath.Replace(FilePathKeyword.ProcessId, Process.GetCurrentProcess().Id.ToString());
            }

            if (filePath.Contains(FilePathKeyword.Date))
            {
                filePath = filePath.Replace(FilePathKeyword.Date, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            if (filePath.Contains(FilePathKeyword.Time))
            {
                filePath = filePath.Replace(FilePathKeyword.Time, DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture));
            }

            if (filePath.Contains(FilePathKeyword.AppData))
            {
                filePath = filePath.Replace(FilePathKeyword.AppData, dataDirectory);
            }

            if (filePath.Contains(FilePathKeyword.AppDataLocal))
            {
                var dataDirectoryLocal = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserLocal,
                                                                                                 _assembly.Company(),
                                                                                                 _assembly.Product())
                                                            : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserLocal);


                filePath = filePath.Replace(FilePathKeyword.AppDataLocal, dataDirectoryLocal);
            }

            if (filePath.Contains(FilePathKeyword.AppDataRoaming))
            {
                var dataDirectoryRoaming = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserRoaming,
                                                                                                   _assembly.Company(),
                                                                                                   _assembly.Product())
                                                             : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserRoaming);


                filePath = filePath.Replace(FilePathKeyword.AppDataRoaming, dataDirectoryRoaming);
            }

            if (filePath.Contains(FilePathKeyword.AppDataMachine))
            {
                var dataDirectoryMachine = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.Machine,
                                                                                                   _assembly.Company(),
                                                                                                   _assembly.Product())
                                                             : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.Machine);

                filePath = filePath.Replace(FilePathKeyword.AppDataMachine, dataDirectoryMachine);
            }

            if (filePath.Contains(FilePathKeyword.AppDir))
            {
                filePath = filePath.Replace(FilePathKeyword.AppDir, AppDomain.CurrentDomain.BaseDirectory);
            }

            if (filePath.Contains(FilePathKeyword.WorkDir))
            {
                filePath = filePath.Replace(FilePathKeyword.WorkDir, Directory.GetCurrentDirectory());
            }

            filePath = IO.Path.GetFullPath(filePath, dataDirectory);

            if (!filePath.EndsWith(".log"))
            {
                filePath += ".log";
            }

            return filePath;
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        protected override async Task WriteBatchAsync(System.Collections.Generic.List<LogBatchEntry> batchEntries)
        {
            try
            {
                var filePath = FilePath;

                var directoryName = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists && (fileInfo.Length / 1024 >= MaxSizeInKiloBytes))
                {
                    CreateCopyOfCurrentLogFile(FilePath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        foreach (var batchEntry in batchEntries)
                        {
                            var message = FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, batchEntry.Data, batchEntry.Time);

                            writer.WriteLine(message);
                        }

                        writer.Flush();
                    }
                }
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        private void Initialize(bool initFilePath, Assembly assembly = null)
        {
            _assembly = assembly ?? AssemblyHelper.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

            if (initFilePath && string.IsNullOrWhiteSpace(_filePath))
            {
                _filePath = DetermineFilePath(FilePathKeyword.AutoLogFileName);
            }
        }

        private void CreateCopyOfCurrentLogFile(string filePath)
        {
            for (int i = 1; i < 999; i++)
            {
                var possibleFilePath = string.Format("{0}.{1:000}", filePath, i);
                if (!File.Exists(possibleFilePath))
                {
                    File.Move(filePath, possibleFilePath);
                }
            }
        }
        #endregion
    }
}

#endif