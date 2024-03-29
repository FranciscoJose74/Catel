﻿namespace Catel.MVVM.Navigation
{
    using System;
    using System.Windows.Navigation;

    /// <summary>
    /// Navigation event args extensions.
    /// </summary>
    public static class NavigationEventArgsExtensions
    {
        /// <summary>
        /// Determines whether the specified string is a navigation to an external source.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns><c>true</c> if the uri is a navigation to an external source; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public static bool IsNavigationToExternal(this Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            return IsNavigationToExternal(uri.ToString());
        }

        /// <summary>
        /// Determines whether the specified string is a navigation to an external source.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <returns><c>true</c> if the uri is a navigation to an external source; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uriString"/> is <c>null</c> or whitespace.</exception>
        public static bool IsNavigationToExternal(this string uriString)
        {
            Argument.IsNotNullOrWhitespace("uriString", uriString);

            return uriString.Contains("app://external");
        }

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
            ArgumentNullException.ThrowIfNull(e);
            ArgumentNullException.ThrowIfNull(viewType);

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
            ArgumentNullException.ThrowIfNull(e);
            ArgumentNullException.ThrowIfNull(viewType);

            var uriString = GetUriWithoutQueryInfo(e);
            return IsNavigationForView(uriString, viewType);
        }

        /// <summary>
        /// Determines whether the navigation is for the specified view model.
        /// </summary>
        /// <param name="uriString">The uri string instance containing the event data.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns><c>true</c> if the navigation is for the specified view model; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="uriString"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static bool IsNavigationForView(this string uriString, Type viewType)
        {
            Argument.IsNotNullOrWhitespace("uriString", uriString);
            ArgumentNullException.ThrowIfNull(viewType);

            return uriString.ContainsIgnoreCase(viewType.Name + ".xaml");
        }

        /// <summary>
        /// Gets the URI from the navigating context.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        /// <returns>The uri.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        public static string GetUriWithoutQueryInfo(this NavigatingCancelEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            var uriString = UriExtensions.GetSafeUriString(e.Uri);
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
            ArgumentNullException.ThrowIfNull(e);

            var uriString = UriExtensions.GetSafeUriString(e.Uri);
            return uriString;
        }

        /// <summary>
        /// Gets the URI from the navigated context.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The uri.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uri" /> is <c>null</c> or whitespace.</exception>
        public static string GetUriWithoutQueryInfo(this string uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            var uriString = uri;
            return uriString;
        }
    }
}
