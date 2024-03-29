﻿namespace Catel.Data
{
    using System;

    /// <summary>
    /// An interface that allows public editing of <see cref="ModelBase"/> instances using the <c>GetValue</c>
    /// and <c>SetValue</c> methods.
    /// </summary>
    public interface IModelEditor
    {
        /// <summary>
        /// Checks whether the specified property name is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns><c>true</c> if the specified property is available; otherwise <c>false</c>.</returns>
        bool IsPropertyRegistered(string propertyName);

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue GetValue<TValue>(string propertyName);

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        void SetValue<TValue>(string propertyName, TValue value);

        /// <summary>
        /// Gets the value in the fastest way possible without doing sanity checks.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value.</returns>
        /// <remarks>
        /// Note that this method does not do any sanity checks. Use at your own risk!
        /// </remarks>
        TValue GetValueFastButUnsecure<TValue>(string propertyName);

        /// <summary>
        /// Sets the value in the fastest way possible without doing sanity checks.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The value.</returns>
        /// <remarks>
        /// Note that this method does not do any sanity checks. Use at your own risk!
        /// </remarks>
        void SetValueFastButUnsecure<TValue>(string propertyName, TValue value);
    }
}
