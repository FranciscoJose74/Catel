﻿namespace Catel.Data
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// IModelBase that the <see cref="ModelBase"/> must implement to easily pass objects to methods as non-generic.
    /// </summary>
    /// <remarks>
    /// This interface defines all the non-generic interfaces that the <see cref="ModelBase"/> class implements.
    /// </remarks>
    public interface IModel : INotifyPropertyChanged, IAdvancedEditableObject, IModelEditor, IModelSerialization, IFreezable
    {
        /// <summary>
        /// Gets the name of the object. By default, this is the name of the inherited class.
        /// </summary>
        /// <value>The name of the key.</value>
        string KeyName { get; }

        /// <summary>
        /// Gets a value indicating whether this object is dirty.
        /// </summary>
        /// <value><c>true</c> if this object is dirty; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets a value indicating whether the object is currently in an edit session, started by the <see cref="IEditableObject.BeginEdit"/> method.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently in an edit session; otherwise, <c>false</c>.
        /// </value>
        bool IsInEditSession { get; }

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        object? GetDefaultValue(string name);

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        TValue GetDefaultValue<TValue>(string name);

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        Type GetPropertyType(string name);
    }
}
