﻿namespace Catel.MVVM.Providers
{
    using System;

    /// <summary>
    /// Extension methods to safely interact with logic from inside views.
    /// </summary>
    public static class LogicExtensions
    {
        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="action">The action that will set the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static void SetValue<TLogic>(this LogicBase logic, Action<TLogic> action)
            where TLogic : LogicBase
        {
            ArgumentNullException.ThrowIfNull(action);

            if (logic is null)
            {
                return;
            }

            action((TLogic)logic);
        }

        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="function">The function that will get the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="function"/> is <c>null</c>.</exception>
        public static TValue GetValue<TLogic, TValue>(this LogicBase logic, Func<TLogic, TValue> function)
            where TLogic : LogicBase
        {
            return GetValue(logic, function, default)!;
        }

        /// <summary>
        /// Sets the value of the logic property.
        /// </summary>
        /// <param name="logic">The logic, can be <c>null</c> so the caller don't have to check for this.</param>
        /// <param name="function">The function that will get the actual value, will only be executed if <paramref name="logic"/> is not <c>null</c>.</param>
        /// <param name="defaultValue">The default value to return if the logic is not available.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="function"/> is <c>null</c>.</exception>
        public static TValue GetValue<TLogic, TValue>(this LogicBase logic, Func<TLogic, TValue> function, TValue defaultValue)
            where TLogic : LogicBase
        {
            ArgumentNullException.ThrowIfNull(function);

            if (logic is null)
            {
                return defaultValue;
            }

            return function((TLogic) logic);
        }
    }
}
