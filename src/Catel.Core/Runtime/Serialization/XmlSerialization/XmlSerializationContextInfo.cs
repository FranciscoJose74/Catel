﻿namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Catel.Collections;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    public class XmlSerializationContextInfo : SerializationContextInfoBase<XmlSerializationContextInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="xmlWriter">The xml writer.</param>
        /// <param name="model">The model, is allowed to be null for value types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlWriter" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(XmlWriter xmlWriter, object model)
        {
            XmlWriter = xmlWriter;
            IsRootObject = xmlWriter.WriteState == WriteState.Start;
            AllowCustomXmlSerialization = true;
            KnownTypes = new HashSet<Type>();
            Model = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlReader" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        public XmlSerializationContextInfo(XmlReader xmlReader, object model)
        {
            XmlReader = xmlReader;
            IsRootObject = xmlReader.NodeType == XmlNodeType.None;
            AllowCustomXmlSerialization = true;
            KnownTypes = new HashSet<Type>();
            Model = model;
        }

        /// <summary>
        /// Gets whether this object is the root object of the xml graph.
        /// </summary>
        public bool IsRootObject { get; private set; }

        /// <summary>
        /// Gets the xml writer.
        /// </summary>
        public XmlWriter? XmlWriter { get; private set; }

        /// <summary>
        /// Gets the xml reader.
        /// </summary>
        public XmlReader? XmlReader { get; private set; }

        /// <summary>
        /// Gets the list of known types from the current stack.
        /// </summary>
        /// <value>
        /// The known types.
        /// </value>
        public HashSet<Type> KnownTypes { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public object? Model { get; private set; }

        /// <summary>
        /// Gets or sets whether custom xml serialization is allowed via the <see cref="ICustomXmlSerializable"/> interface.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public bool AllowCustomXmlSerialization { get; set; }

        protected override void OnContextUpdated(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.OnContextUpdated(context);

            var parentContext = context?.Parent;

            Debug.Assert(!ReferenceEquals(context, parentContext));

            var parentKnownTypes = parentContext?.Context?.KnownTypes;
            if (parentKnownTypes is not null)
            {
                // Note: sometimes Catel re-uses the types, but in that case the types won't be added
                // as duplicates anyway
                KnownTypes.AddRange(parentKnownTypes);
            }
        }
    }
}
