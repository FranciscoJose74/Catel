﻿namespace Catel.Windows
{
    using System;
    using System.Windows;
    using Logging;

    /// <summary>
    /// Resource helper class.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ensures that the pack URI is allowed. Sometimes, when no application object is instantiated
        /// yet, the pack URI is not allowed. This method takes care of that.
        /// </summary>
        public static void EnsurePackUriIsAllowed()
        {
            if (!UriParser.IsKnownScheme("pack"))
            {
                Log.Debug("Pack uri is not yet allowed, adding it as known scheme");

                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
            }
        }

        /// <summary>
        /// Gets the resource URI for the specified resource and assembly name. The uri will
        /// be created like the following examples:
        /// <list type="bullet">
        ///   <item>
        ///     <description>pack://application:,,,/[RESOURCEURI]</description>
        ///   </item>
        ///   <item>
        ///     <description>pack://application:,,,/[ASSEMBLY];component/[RESOURCEURI]</description>
        ///   </item>
        /// </list>
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="shortAssemblyName">Name of the assembly. If <c>null</c> or empty, the current application will be used.
        /// If used, this must be the short name of the assembly.</param>
        /// <returns>The resource uri.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceUri"/> is <c>null</c> or whitespace.</exception>
        public static string GetResourceUri(string resourceUri, string? shortAssemblyName = null)
        {
            Argument.IsNotNullOrWhitespace("resourceUri", resourceUri);

            while (resourceUri.StartsWith("/"))
            {
                resourceUri = resourceUri.Remove(0, 1);
            }

            if (string.IsNullOrEmpty(shortAssemblyName))
            {
                // Current app resource
                return string.Format("pack://application:,,,/{0}", resourceUri);
            }

            return string.Format("pack://application:,,,/{0};component/{1}", shortAssemblyName, resourceUri);
        }

        /// <summary>
        /// Determines whether the specified uri is pointing to a valid xaml file.
        /// </summary>
        /// <returns><c>true</c> if the specified uri is pointing to a valid xaml file; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Note that the <paramref name="uriString"/> must be a valid resource URI. The <see cref="GetResourceUri(string, string)"/> can be used to
        /// easily create a resource URI.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="uriString"/> is <c>null</c> or whitespace.</exception>
        public static bool XamlPageExists(string uriString)
        {
            Argument.IsNotNullOrWhitespace("uriString", uriString);

            EnsurePackUriIsAllowed();

            return XamlPageExists(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Determines whether the specified uri is pointing to a valid xaml file.
        /// </summary>
        /// <returns><c>true</c> if the specified uri is pointing to a valid xaml file; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Note that the <paramref name="uri"/> must be a valid resource URI. The <see cref="GetResourceUri(string, string)"/> can be used to
        /// easily create a resource URI.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public static bool XamlPageExists(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            try
            {
                return Application.GetResourceStream(uri) is not null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
