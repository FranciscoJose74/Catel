﻿namespace Catel
{
    using System;
    using Configuration;
    using Data;
    using IoC;
    using Messaging;
    using Runtime.Serialization;
    using Runtime.Serialization.Xml;
    using Services;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public class CoreModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            // No need to clean the small boxing caches
            BoxingCache<bool>.Default.CleanUpInterval = TimeSpan.Zero;

            serviceLocator.RegisterType<ILanguageService, LanguageService>();
            serviceLocator.RegisterType<IAppDataService, AppDataService>();
            serviceLocator.RegisterInstance<IMessageMediator>(MessageMediator.Default);
            serviceLocator.RegisterType<IDispatcherService, ShimDispatcherService>();

            serviceLocator.RegisterType<IValidatorProvider, AttributeValidatorProvider>();

            serviceLocator.RegisterType<IDataContractSerializerFactory, DataContractSerializerFactory>();
            serviceLocator.RegisterType<IXmlSerializer, XmlSerializer>();
            serviceLocator.RegisterType<IXmlNamespaceManager, XmlNamespaceManager>();
            serviceLocator.RegisterType<ISerializationManager, SerializationManager>();
            serviceLocator.RegisterType<Catel.Runtime.Serialization.IObjectAdapter, Catel.Runtime.Serialization.ObjectAdapter>();
            serviceLocator.RegisterType<Catel.Data.IObjectAdapter, Catel.Data.ExpressionTreeObjectAdapter>();

            serviceLocator.RegisterType<ISerializer, XmlSerializer>();
            serviceLocator.RegisterTypeWithTag<ISerializationContextInfoFactory, XmlSerializationContextInfoFactory>(typeof(XmlSerializer));

            serviceLocator.RegisterType<IModelEqualityComparer, ModelEqualityComparer>();
            serviceLocator.RegisterType<IConfigurationService, ConfigurationService>();
            serviceLocator.RegisterType<IObjectConverterService, ObjectConverterService>();
            serviceLocator.RegisterType<IRollingInMemoryLogService, RollingInMemoryLogService>();
        }
    }
}
