﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationEventArgsExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

#if !NETFX_CORE
    using System.Windows.Navigation;
#else
    using global::Windows.UI.Xaml.Navigation;
#endif

    /// <summary>
    /// Navigation event args extensions.
    /// </summary>
    public static class NavigationEventArgsExtensions
    {
        /// <summary>
        /// Determines whether the navigation is for the specified view.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns><c>true</c> if the navigation is for the specified view model; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static bool IsNavigationForView(this NavigatingCancelEventArgs e, Type viewType)
        {
            Argument.IsNotNull("e", e);
            Argument.IsNotNull("viewType", viewType);

            var uriString = GetUriWithoutQueryInfo(e);
            return IsNavigationForView(uriString, viewType);
        }

        /// <summary>
        /// Determines whether the navigation is for the specified view model.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns><c>true</c> if the navigation is for the specified view model; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static bool IsNavigationForView(this NavigationEventArgs e, Type viewType)
        {
            Argument.IsNotNull("e", e);
            Argument.IsNotNull("viewType", viewType);

            var uriString = GetUriWithoutQueryInfo(e);
            return IsNavigationForView(uriString, viewType);
        }

        /// <summary>
        /// Determines whether the navigation is for the specified view model.
        /// </summary>
        /// <param name="uriString">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns><c>true</c> if the navigation is for the specified view model; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="uriString"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static bool IsNavigationForView(string uriString, Type viewType)
        {
            Argument.IsNotNullOrWhitespace("uriString", uriString);
            Argument.IsNotNull("viewType", viewType);

#if NETFX_CORE
            return string.Equals(uriString, viewType.FullName, StringComparison.OrdinalIgnoreCase);
#else
            var lowerUri = uriString.ToLowerInvariant();
            var lowerViewTypeName = viewType.Name.ToLowerInvariant();

            return lowerUri.Contains(lowerViewTypeName + ".xaml");
#endif
        }

        /// <summary>
        /// Gets the URI from the navigating context.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        /// <returns>The uri.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        public static string GetUriWithoutQueryInfo(this NavigatingCancelEventArgs e)
        {
            Argument.IsNotNull("e", e);

#if NETFX_CORE
            string uriString = e.SourcePageType.FullName;
#else
            string uriString = GetSafeUriString(e.Uri);
#endif

            return uriString;
        }

        /// <summary>
        /// Gets the URI from the navigated context.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        /// <returns>The uri.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        public static string GetUriWithoutQueryInfo(this NavigationEventArgs e)
        {
            Argument.IsNotNull("e", e);

#if NETFX_CORE
            string uriString = e.SourcePageType.FullName;
#else
            string uriString = GetSafeUriString(e.Uri);
#endif

            return uriString;
        }

        private static string GetSafeUriString(Uri uri)
        {
            Argument.IsNotNull("uri", uri);

            return IsAbsoluteUrl(uri.ToString()) ? uri.AbsoluteUri : uri.OriginalString;
        }

        private static bool IsAbsoluteUrl(string url)
        {
            Argument.IsNotNull("url", url);

            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}