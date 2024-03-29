﻿namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// The serialization event args.
    /// </summary>
    public class SerializationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationEventArgs"/> class.
        /// </summary>
        /// <param name="serializationContext">The serialization context.</param>
        public SerializationEventArgs(ISerializationContext serializationContext)
        {
            SerializationContext = serializationContext;
        }

        /// <summary>
        /// Gets the serialization context.
        /// </summary>
        /// <value>The serialization context.</value>
        public ISerializationContext SerializationContext { get; private set; }
    }
}
