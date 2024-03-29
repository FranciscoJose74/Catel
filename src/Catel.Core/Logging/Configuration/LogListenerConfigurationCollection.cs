﻿namespace Catel.Logging
{
    using System;
    using System.Configuration;

    /// <summary>
    /// The log listeners configuration collection.
    /// </summary>
    public sealed class LogListenerConfigurationCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// The item element name.
        /// </summary>
        private const string ItemElementName = "listener";

        /// <summary>
        /// The collection type.
        /// </summary>
        /// <value>The type of the collection.</value>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection.</returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        /// <summary>
        /// Creates a new ConfigurationElement.
        /// </summary>
        /// <returns>A new <see cref="System.Configuration.ConfigurationElement" />.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogListenerConfiguration();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>An <see cref="object" /> that acts as the key for the specified <see cref="System.Configuration.ConfigurationElement" />.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogListenerConfiguration)element).Type;
        }

        /// <summary>
        /// Determines whether the given <param ref="elementName" /> is the collection element name.
        /// </summary>
        /// <param name="elementName">The element name</param>
        /// <returns><c>true</c> if is the element name, otherwise <c>false</c>.</returns>
        protected override bool IsElementName(string elementName)
        {
            return string.Equals(elementName, ItemElementName, StringComparison.Ordinal);
        }
    }
}
