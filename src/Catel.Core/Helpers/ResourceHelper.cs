﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;

    /// <summary>
    /// Resource helper class to read resource files.
    /// </summary>
    public static class ResourceHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ILanguageService _languageService = ServiceLocator.Default.ResolveType<ILanguageService>();

        /// <summary>
        /// Gets the string from the specified resource file.
        /// </summary>
        /// <param name="callingType">Type of the calling.</param>
        /// <param name="resourceFile">The resource file.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">The <paramref name="resourceFile"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName"/> is <c>null</c> or whitespace.</exception>
        public static string GetString(Type callingType, string resourceFile, string resourceName)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            return GetString(resourceName);
        }

        /// <summary>
        /// Gets the string from the specified resource file.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c> or whitespace.</exception>
        public static string GetString(string resourceName)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            return _languageService.GetString(resourceName);
        }

        /// <summary>
        /// Extracts the embedded resource and reads it as a string.
        /// </summary>
        /// <param name="assembly">The assembly to read the resource from.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The embedded resource as a string.</returns>
        public static string ExtractEmbeddedResource(this Assembly assembly, string resourceName)
        {
            using (var memoryStream = new MemoryStream())
            {
                ExtractEmbeddedResource(assembly, resourceName, memoryStream);

                if (memoryStream.Length == 0)
                {
                    return null;
                }

                memoryStream.Position = 0L;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Extracts the embedded resource and writes it to the target stream.
        /// </summary>
        /// <param name="assembly">The assembly to read the resource from.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="targetStream">The target stream to write the resource to.</param>
        public static void ExtractEmbeddedResource(this Assembly assembly, string resourceName, Stream targetStream)
        {
            Log.Debug("Extracting embedded resource '{0}' from assembly '{1}'", resourceName, assembly.FullName);

            using (var resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    var warning = new StringBuilder();
                    warning.AppendLine($"Failed to extract embedded resource '{resource}', possible names:");
                    
                    foreach (var name in assembly.GetManifestResourceNames())
                    {
                        warning.AppendLine($"  * {name}");
                    }

                    Log.Warning(warning.ToString());
                    return;
                }

                resource.CopyTo(targetStream);
            }
        }
    }
}