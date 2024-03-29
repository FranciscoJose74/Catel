﻿namespace Catel.Runtime.Serialization.Xml
{
    /// <summary>
    /// Possible xml serializer optimalization modes
    /// </summary>
    public enum XmlSerializerOptimalizationMode
    {
        /// <summary>
        /// If pretty xml is required (for display reasons), pick this one.
        /// </summary>
        PrettyXml,

        /// <summary>
        /// If pretty xml is required (for display reasons), pick this one. This will remove all namespaces instead of only the root ones.
        /// </summary>
        PrettyXmlAgressive,

        /// <summary>
        /// If duplicate namespaces are irrelevant, pick this for speed.
        /// </summary>
        Performance
    }
}
