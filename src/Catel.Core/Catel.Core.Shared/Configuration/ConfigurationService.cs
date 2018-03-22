﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using Runtime.Serialization;
    using Services;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using Data;
    using Catel.Logging;
    using Runtime.Serialization.Xml;
#if PCL
    // Not supported
#elif NETFX_CORE
    using Windows.Storage;
#else
    using System.Configuration;
    using System.Linq;
    using Path = IO.Path;
#endif

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// <para />
    /// This default implementation writes to the
    /// </summary>
    public partial class ConfigurationService : IConfigurationService
    {
        private static readonly string DefaultLocalConfigFilePath = Path.Combine(Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserLocal), "configuration.xml");

        private static readonly string DefaultRoamingConfigFilePath = Path.Combine(Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming), "configuration.xml");

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ISerializationManager _serializationManager;
        private readonly IObjectConverterService _objectConverterService;
        private readonly ISerializer _serializer;

#if NET || NETSTANDARD
        private DynamicConfiguration _localConfiguration;
        private DynamicConfiguration _roamingConfiguration;

        private string _localConfigFilePath;
        private string _roamingConfigFilePath;
#elif ANDROID
        private readonly global::Android.Content.ISharedPreferences _preferences =
            global::Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(global::Android.App.Application.Context);
#endif

        private bool _suspendNotifications = false;
        private bool _hasPendingNotifications = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        public ConfigurationService(ISerializationManager serializationManager,
            IObjectConverterService objectConverterService, IXmlSerializer serializer)
            : this(serializationManager, objectConverterService, (ISerializer)serializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        public ConfigurationService(ISerializationManager serializationManager, 
            IObjectConverterService objectConverterService, ISerializer serializer)
        {
            Argument.IsNotNull("serializationManager", serializationManager);
            Argument.IsNotNull("objectConverterService", objectConverterService);
            Argument.IsNotNull("serializer", serializer);

            _serializationManager = serializationManager;
            _objectConverterService = objectConverterService;
            _serializer = serializer;

#if NET || NETSTANDARD
            SetLocalConfigFilePath(DefaultLocalConfigFilePath);
            SetRoamingConfigFilePath(DefaultRoamingConfigFilePath);
#endif
        }

#region Events
        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
#endregion

#region Methods
        /// <summary>
        /// Suspends the notifications of this service until the returned object is disposed.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public IDisposable SuspendNotifications()
        {
            return new DisposableToken<ConfigurationService>(this,
                x =>
                {
                    x.Instance._suspendNotifications = true;
                },
                x =>
                {
                    x.Instance._suspendNotifications = false;
                    if (x.Instance._hasPendingNotifications)
                    {
                        x.Instance.RaiseConfigurationChanged(ConfigurationContainer.Roaming, string.Empty, string.Empty);
                        x.Instance._hasPendingNotifications = false;
                    }
                });
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public T GetValue<T>(ConfigurationContainer container, string key, T defaultValue = default(T))
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            try
            {
                if (!ValueExists(container, key))
                {
                    return defaultValue;
                }

                var value = GetValueFromStore(container, key);
                if (value == null)
                {
                    return defaultValue;
                }

                // ObjectConverterService doesn't support object, but just return the value as is
                if (typeof(T) == typeof(object))
                {
                    return (T)(object)value;
                }

                return (T)_objectConverterService.ConvertFromStringToObject(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to retrieve configuration value '{container}.{key}', returning default value");

                return defaultValue;
            }
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public void SetValue(ConfigurationContainer container, string key, object value)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var originalKey = key;
            key = GetFinalKey(key);

            var stringValue = _objectConverterService.ConvertFromObjectToString(value, CultureInfo.InvariantCulture);
            var existingValue = GetValueFromStore(container, key);

            SetValueToStore(container, key, stringValue);

            if (!string.Equals(stringValue, existingValue))
            {
                RaiseConfigurationChanged(container, originalKey, value);
            }
        }

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public bool IsValueAvailable(ConfigurationContainer container, string key)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            return ValueExists(container, key);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public void InitializeValue(ConfigurationContainer container, string key, object defaultValue)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            if (!IsValueAvailable(container, key))
            {
                SetValue(container, key, defaultValue);
            }
        }

#if NET || NETSTANDARD
        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        public void SetRoamingConfigFilePath(string filePath)
        {
            Argument.IsNotNullOrEmpty(nameof(filePath), filePath);

            _roamingConfigFilePath = filePath;

            try
            {
                if (File.Exists(_roamingConfigFilePath))
                {
                    using (var fileStream = new FileStream(_roamingConfigFilePath, FileMode.Open))
                    {
                        _roamingConfiguration = SavableModelBase<DynamicConfiguration>.Load(fileStream, _serializer);
                    }
                }
                else
                {
                    _roamingConfiguration?.SaveAsXml(_roamingConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load roaming configuration, using default settings");

                _roamingConfigFilePath = DefaultRoamingConfigFilePath;
            }

            if (_roamingConfiguration == null)
            {
                _roamingConfiguration = new DynamicConfiguration();
            }
        }

        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        public void SetLocalConfigFilePath(string filePath)
        {
            Argument.IsNotNullOrEmpty(nameof(filePath), filePath);

            _localConfigFilePath = filePath;

            try
            {
                if (File.Exists(_localConfigFilePath))
                {
                    using (var fileStream = new FileStream(_localConfigFilePath, FileMode.Open))
                    {
                        _localConfiguration = SavableModelBase<DynamicConfiguration>.Load(fileStream, _serializer);
                    }
                }
                else
                {
                    _localConfiguration?.SaveAsXml(_localConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load local configuration, using default settings");

                _localConfigFilePath = DefaultLocalConfigFilePath;
            }

            if (_localConfiguration == null)
            {
                _localConfiguration = new DynamicConfiguration();
            }
        }
#endif

        /// <summary>
        /// Determines whether the specified key value exists in the configuration.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
        protected virtual bool ValueExists(ConfigurationContainer container, string key)
        {
#if PCL || (XAMARIN && !ANDROID)
            throw Log.ErrorAndCreateException<NotSupportedInPlatformException>("No configuration objects available");
#elif ANDROID
            return _preferences.Contains(key);
#elif NETFX_CORE
            var settings = GetSettingsContainer(container);
            return settings.Values.ContainsKey(key);
#else
            var settings = GetSettingsContainer(container);
            return settings.IsConfigurationValueSet(key);
#endif
        }

        /// <summary>
        /// Gets the value from the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected virtual string GetValueFromStore(ConfigurationContainer container, string key)
        {
#if PCL || (XAMARIN && !ANDROID)
            throw Log.ErrorAndCreateException<NotSupportedInPlatformException>("No configuration objects available");
#elif ANDROID
            return _preferences.GetString(key, null);
#elif NETFX_CORE
            var settings = GetSettingsContainer(container);
            return (string)settings.Values[key];
#else
            var settings = GetSettingsContainer(container);
            return settings.GetConfigurationValue<string>(key, string.Empty);
#endif
        }

        /// <summary>
        /// Sets the value to the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetValueToStore(ConfigurationContainer container, string key, string value)
        {
#if PCL || (XAMARIN && !ANDROID)
            throw Log.ErrorAndCreateException<NotSupportedInPlatformException>("No configuration objects available");
#elif ANDROID
            _preferences.Edit()
                        .PutString(key, value)
                        .Apply();
#elif NETFX_CORE
            var settings = GetSettingsContainer(container);
            settings.Values[key] = value;
#else
            var settings = GetSettingsContainer(container);

            if (!settings.IsConfigurationValueSet(key))
            {
                settings.RegisterConfigurationKey(key);
            }

            settings.SetConfigurationValue(key, value);

            string fileName = string.Empty;

            switch (container)
            {
                case ConfigurationContainer.Local:
                    fileName = _localConfigFilePath;
                    break;

                case ConfigurationContainer.Roaming:
                    fileName = _roamingConfigFilePath;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("container");
            }

            settings.SaveAsXml(fileName);
#endif
        }

        /// <summary>
        /// Gets the final key. This method allows customization of the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetFinalKey(string key)
        {
            key = key.Replace(" ", "_");

            return key;
        }

        private void RaiseConfigurationChanged(ConfigurationContainer container, string key, object value)
        {
            if (_suspendNotifications)
            {
                _hasPendingNotifications = true;
                return;
            }

            ConfigurationChanged.SafeInvoke(this, () => new ConfigurationChangedEventArgs(container, key, value));
        }
#endregion
    }
}