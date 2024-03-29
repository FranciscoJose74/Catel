﻿namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="System.Windows.Application"/> class.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Gets the currently active window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>
        /// The active window of the application or null in case of none window is opened.
        /// </returns>
        public static System.Windows.Window? GetActiveWindow(this System.Windows.Application application)
        {
            System.Windows.Window? activeWindow = null;

            // CTL-687: Only allow windows that have an actual size (been shown at least once)
            Func<System.Windows.Window, bool> predicate = x => x.IsValidAsOwnerWindow();

            if (application is not null && application.Windows.Count > 0)
            {
                var windowList = new List<System.Windows.Window>(application.Windows.Cast<System.Windows.Window>());

                activeWindow = windowList.FirstOrDefault(x => x.IsActive && predicate(x));
                if (activeWindow is null && windowList.Count == 1 && windowList[0].Topmost)
                {
                    activeWindow = windowList[0];
                }

                if (activeWindow is null)
                {
                    activeWindow = windowList.LastOrDefault(predicate);
                }
            }

            return activeWindow;
        }
    }
}
