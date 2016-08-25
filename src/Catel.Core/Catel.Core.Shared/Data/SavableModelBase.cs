﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using Logging;
    using Catel.Runtime.Serialization;

#if NET
    using System.Runtime.Serialization;
#elif NETFX_CORE
    using Windows.Storage.Streams;
#elif PCL
    // Not supported in Portable Class Library
#else
    using System.IO.IsolatedStorage;
#endif

    /// <summary>
    /// Abstract class that makes the <see cref="ModelBase"/> serializable.
    /// </summary>
    /// <typeparam name="T">Type that the class should hold (same as the defined type).</typeparam>
#if NET
    [Serializable]
#endif
    public abstract class SavableModelBase<T> : ModelBase, ISavableModel
        where T : class
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SavableModelBase{T}"/> class.
        /// </summary>
        protected SavableModelBase()
        {
#if NET
            Mode = SerializationMode.Binary;
#else
            Mode = SerializationMode.Xml;
#endif
        }

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="SavableModelBase{T}"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo object, null if this is the first time construction.</param>
        /// <param name="context">StreamingContext object, simple pass a default new StreamingContext() if this is the first time construction.</param>
        /// <remarks>
        /// Call this method, even when constructing the object for the first time (thus not deserializing).
        /// </remarks>
        protected SavableModelBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets the bytes of the current binary serialized data object.
        /// </summary>
        /// <value>The bytes.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        [ObsoleteEx(TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public byte[] Bytes
        {
            get { return ToByteArray(null); }
        }

        /// <summary>
        /// Gets the <see cref="SerializationMode"/> of this object.
        /// </summary>
        /// <value>The serialization mode.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public SerializationMode Mode { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the object to and xml object.
        /// </summary>
        /// <returns><see cref="XDocument"/> containing the serialized data.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = "ToXml(ISerializationConfiguration)",
             TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public XDocument ToXml()
        {
            return ToXml(null);
        }

        /// <summary>
        /// Serializes the object to and xml object.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        ///   <see cref="XDocument" /> containing the serialized data.
        /// </returns>
        public XDocument ToXml(ISerializationConfiguration configuration)
        {
            using (var memoryStream = new MemoryStream())
            {
                Save(memoryStream, SerializationMode.Xml, configuration);

                memoryStream.Position = 0L;

                using (var xmlReader = XmlReader.Create(memoryStream))
                {
                    return XDocument.Load(xmlReader);
                }
            }
        }

        /// <summary>
        /// Serializes the object to a byte array.
        /// </summary>
        /// <returns>Byte array containing the serialized data.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = "ToByteArray(ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public byte[] ToByteArray()
        {
            return ToByteArray(null);
        }

        /// <summary>
        /// Serializes the object to a byte array.
        /// </summary>
        /// <returns>Byte array containing the serialized data.</returns>
        public byte[] ToByteArray(ISerializationConfiguration configuration)
        {
            using (var memoryStream = new MemoryStream())
            {
#if NET
                Save(memoryStream, SerializationMode.Binary, configuration);
#else
                Save(memoryStream, SerializationMode.Xml, configuration);
#endif

                return memoryStream.ToArray();
            }
        }

        #region Saving
#if NET || XAMARIN
        /// <summary>
        /// Saves the object to a file using the default formatting.
        /// </summary>
        /// <param name="fileName">Filename of the file that will contain the serialized data of this object.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "Save(string, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public void Save(string fileName)
        {
            Save(fileName, null);
        }

        /// <summary>
        /// Saves the object to a file using the default formatting.
        /// </summary>
        /// <param name="fileName">Filename of the file that will contain the serialized data of this object.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(string fileName, ISerializationConfiguration configuration)
        {
            Save(fileName, Mode, configuration);
        }

        /// <summary>
        /// Saves the object to a file using a specific formatting.
        /// </summary>
        /// <param name="fileName">Filename of the file that will contain the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "Save(string, SerializationMode, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public void Save(string fileName, SerializationMode mode)
        {
            Save(fileName, mode, null);
        }

        /// <summary>
        /// Saves the object to a file using a specific formatting.
        /// </summary>
        /// <param name="fileName">Filename of the file that will contain the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(string fileName, SerializationMode mode, ISerializationConfiguration configuration)
        {
            var fileInfo = new FileInfo(fileName);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            using (Stream stream = new FileStream(fileName, FileMode.Create))
            {
                Save(stream, mode, configuration);
            }
        }
#elif NETFX_CORE
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        public void Save(IRandomAccessStream fileStream)
        {
            Save((Stream)fileStream, null);
        }
#elif PCL
        // Not supported in Portable Class Library
#else
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        public void Save(IsolatedStorageFileStream fileStream)
        {
            Save((Stream)fileStream, null);
        }
#endif

        /// <summary>
        /// Saves the object to a stream using the default formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "Save(Stream, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public void Save(Stream stream)
        {
            Save(stream, null);
        }

        /// <summary>
        /// Saves the object to a stream using the default formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(Stream stream, ISerializationConfiguration configuration)
        {
            Save(stream, Mode, configuration);
        }

        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "Save(Stream, SerializationMode, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public void Save(Stream stream, SerializationMode mode)
        {
            Save(stream, mode, null);
        }

        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(Stream stream, SerializationMode mode, ISerializationConfiguration configuration)
        {
            Log.Debug("Saving object '{0}' as '{1}'", GetType().Name, mode);

            switch (mode)
            {
#if NET
                case SerializationMode.Binary:
                    var binarySerializer = SerializationFactory.GetBinarySerializer();
                    binarySerializer.Serialize(this, stream, configuration);
                    break;
#endif

                case SerializationMode.Xml:
                    var xmlSerializer = SerializationFactory.GetXmlSerializer();
                    xmlSerializer.Serialize(this, stream, configuration);
                    break;
            }

            Log.Debug("Saved object");

            ClearIsDirtyOnAllChilds();
        }
        #endregion

        #region Loading
#if NETFX_CORE
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load(IRandomAccessStream fileStream, SerializationMode mode)
        {
            return Load<T>((Stream)fileStream, mode, null);
        }
#elif PCL
        // Not supported in Portable Class Library
#elif SILVERLIGHT
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load(IsolatedStorageFileStream fileStream, SerializationMode mode)
        {
            return Load<T>(fileStream, mode, null);
        }
#endif

        /// <summary>
        /// Loads the object from an XmlDocument object.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        public static T Load(XDocument xmlDocument)
        {
            return Load<T>(xmlDocument);
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.</remarks>
        [ObsoleteEx(ReplacementTypeOrMember = "Load(Stream, SerializationMode, ISerializationConfiguration)",
                    TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public static T Load(Stream stream, SerializationMode mode)
        {
            return Load(stream, mode, null);
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load(Stream stream, SerializationMode mode, ISerializationConfiguration configuration)
        {
            return Load<T>(stream, mode, configuration);
        }
        #endregion
        #endregion
    }
}