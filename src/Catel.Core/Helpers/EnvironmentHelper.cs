﻿namespace Catel
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Helper class for environment information.
    /// </summary>
    public static class EnvironmentHelper
    {
        private static readonly Lazy<bool> _hostedByVisualStudio = new Lazy<bool>(() => IsProcessCurrentlyHostedByVisualStudio(false));
        private static readonly Lazy<bool> _hostedBySharpDevelop = new Lazy<bool>(() => IsProcessCurrentlyHostedBySharpDevelop(false));
        private static readonly Lazy<bool> _hostedByExpressionBlend = new Lazy<bool>(() => IsProcessCurrentlyHostedByExpressionBlend(false));

        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByVisualStudio
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedByVisualStudio is null)
                {
                    return false;
                }

                return _hostedByVisualStudio.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by sharp develop.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by sharp develop; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedBySharpDevelop
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedBySharpDevelop is null)
                {
                    return false;
                }

                return _hostedBySharpDevelop.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByExpressionBlend
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedByExpressionBlend is null)
                {
                    return false;
                }

                return _hostedByExpressionBlend.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by any tool, such as visual studio or blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByTool
        {
            get { return IsProcessHostedByVisualStudio || IsProcessHostedByExpressionBlend; }
        }

        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByVisualStudio" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByVisualStudio(bool checkParentProcesses = false)
        {
            return IsHostedByProcess("devenv", checkParentProcesses);
        }

        /// <summary>
        /// Determines whether the process is hosted by sharp develop.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByExpressionBlend" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by sharp develop; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedBySharpDevelop(bool checkParentProcesses = false)
        {
            return IsHostedByProcess("sharpdevelop", checkParentProcesses);
        }

        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByExpressionBlend" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByExpressionBlend(bool checkParentProcesses = false)
        {
            return IsHostedByProcess("blend", checkParentProcesses);
        }

        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByTool" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the current process is hosted by any tool; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByTool(bool checkParentProcesses = false)
        {
            if (IsProcessCurrentlyHostedByVisualStudio(checkParentProcesses))
            {
                return true;
            }

            if (IsProcessCurrentlyHostedBySharpDevelop(checkParentProcesses))
            {
                return true;
            }

            if (IsProcessCurrentlyHostedByExpressionBlend(checkParentProcesses))
            {
                return true;
            }

            return false;
        }

        private static bool IsHostedByProcess(string processName, bool supportParentProcesses = false)
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                if (currentProcess is null)
                {
                    return false;
                }

                var currentProcessName = currentProcess.ProcessName;
                if (supportParentProcesses && currentProcessName.ContainsIgnoreCase("vshost"))
                {
                    return false;
                }

                var isHosted = currentProcessName.StartsWithIgnoreCase(processName);
                return isHosted;
            }
            catch (Exception)
            {
                // Ignore
                return false;
            }
        }
    }
}
