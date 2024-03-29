﻿namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A class to be able to create delegates in both .NET and WinRT.
    /// </summary>
    public static class DelegateHelper
    {
        /// <summary>
        /// Creates a static delegate for the specified method.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="delegateType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is <c>null</c>.</exception>
        public static Delegate CreateDelegate(Type delegateType, MethodInfo methodInfo)
        {
            return CreateDelegate(delegateType, null, methodInfo);
        }

        /// <summary>
        /// Creates a delegate for the specified method and target.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="delegateType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="methodName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks></remarks>
        public static Delegate CreateDelegate(Type delegateType, Type targetType, string methodName)
        {
            Argument.IsNotNullOrWhitespace("methodName", methodName);

            var methodInfo = targetType.GetMethodEx(methodName, BindingFlagsHelper.GetFinalBindingFlags(true, true));
            if (methodInfo is null)
            {
                throw new CatelException($"Could not find method '{targetType.Name}.{methodName}'");
            }

            return CreateDelegate(delegateType, null, methodInfo);
        }

        /// <summary>
        /// Creates a delegate for the specified method and target.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="target">The target. Cannot be <c>null</c> for this method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="delegateType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="methodName"/> is <c>null</c> or whitespace.</exception>
        public static Delegate CreateDelegate(Type delegateType, object target, string methodName)
        {
            Argument.IsNotNullOrWhitespace("methodName", methodName);

            var methodInfo = target.GetType().GetMethodEx(methodName, BindingFlagsHelper.GetFinalBindingFlags(true, true));
            if (methodInfo is null)
            {
                throw new CatelException($"Could not find method '{target.GetType().Name}.{methodName}'");
            }

            return CreateDelegate(delegateType, target, methodInfo);
        }

        /// <summary>
        /// Creates a delegate for the specified method and target.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="target">The target. If <c>null</c>, the method will be assumed static.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="delegateType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is <c>null</c>.</exception>
        public static Delegate CreateDelegate(Type delegateType, object? target, MethodInfo methodInfo)
        {
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }
    }
}
