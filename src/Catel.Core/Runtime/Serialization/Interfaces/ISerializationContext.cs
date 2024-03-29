﻿namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the serialization context used to serialize and deserialize models.
    /// </summary>
    public interface ISerializationContext : IDisposable
    {
        /// <summary>
        /// Gets the model that needs serialization or deserialization.
        /// </summary>
        /// <value>The model.</value>
        object Model { get; set; }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        Type ModelType { get; }

        /// <summary>
        /// Gets the name of the model type, which should be a cached version of <c>ModelType.GetSafeFullName(false);</c>.
        /// </summary>
        /// <value>The name of the model type.</value>
        string ModelTypeName { get; }

        /// <summary>
        /// Gets the depth of the current element being processed.
        /// </summary>
        /// <value>The depth.</value>
        int Depth { get; }

        /// <summary>
        /// Gets the context mode.
        /// </summary>
        /// <value>The context mode.</value>
        SerializationContextMode ContextMode { get; }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        ReferenceManager ReferenceManager { get; }

        /// <summary>
        /// Gets the type stack inside the current scope.
        /// </summary>
        Stack<Type> TypeStack { get; }

        /// <summary>
        /// Gets the configuration used during serialization.
        /// </summary>
        ISerializationConfiguration? Configuration { get; }
    }

    /// <summary>
    /// Interface for the serialization context used to serialize and deserialize models.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the serialization context.</typeparam>
    public interface ISerializationContext<TSerializationContext> : ISerializationContext
        where TSerializationContext : class, ISerializationContextInfo
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        TSerializationContext Context { get; }

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        /// <value>
        /// The parent context.
        /// </value>
        ISerializationContext<TSerializationContext>? Parent { get; }
    }
}
