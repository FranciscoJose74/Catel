﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Data;
    using IoC;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Runtime.Serialization;
    using JsonSerializer = Runtime.Serialization.Json.JsonSerializer;

    /// <summary>
    /// Json extensions.
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly ISerializationManager SerializationManager = ServiceLocator.Default.ResolveType<ISerializationManager>();
        private static readonly IObjectAdapter ObjectAdapter = ServiceLocator.Default.ResolveType<IObjectAdapter>();

        /// <summary>
        /// Converters the specified model to a json string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ToJson(this ModelBase model, ISerializationConfiguration configuration = null)
        {
            var jsonSerializer = new JsonSerializer(SerializationManager, TypeFactory.Default, ObjectAdapter);

            using (var stream = new MemoryStream())
            {
                jsonSerializer.Serialize(model, stream, configuration);

                stream.Position = 0L;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Creates a json reader with the right configuration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The json reader.
        /// </returns>
        public static JsonReader CreateReader(this JToken token, ISerializationConfiguration configuration)
        {
            var reader = token.CreateReader();
            reader.Culture = configuration.Culture;

            return reader;
        }
    }
}