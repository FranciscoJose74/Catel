﻿namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Reflection;

    /// <summary>
    /// Factory responsible for creating IoC components.
    /// </summary>
    public static class IoCFactory
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly object _lockObject = new object();

        private static Func<IServiceLocator> _createServiceLocatorFunc;
        private static Func<IServiceLocator, IDependencyResolver> _createDependencyResolverFunc;
        private static Func<IServiceLocator, ITypeFactory> _createTypeFactoryFunc;

        private static List<Type>? _serviceLocatorInitializers; 

        /// <summary>
        /// Initializes static members of the <see cref="IoCFactory"/> class.
        /// </summary>
        static IoCFactory()
        {
            _createServiceLocatorFunc = () => new ServiceLocator();
            _createDependencyResolverFunc = serviceLocator => new CatelDependencyResolver(serviceLocator);
            _createTypeFactoryFunc = serviceLocator => new TypeFactory(serviceLocator);

            TypeCache.AssemblyLoaded += OnAssemblyLoaded;
        }

        /// <summary>
        /// Gets or sets the create service locator function.
        /// </summary>
        /// <value>The create service locator function.</value>
        public static Func<IServiceLocator> CreateServiceLocatorFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createServiceLocatorFunc;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _createServiceLocatorFunc = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the create dependency resolverfunction.
        /// </summary>
        /// <value>The create dependency resolver function.</value>
        public static Func<IServiceLocator, IDependencyResolver> CreateDependencyResolverFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createDependencyResolverFunc;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _createDependencyResolverFunc = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the create default service locator function.
        /// </summary>
        /// <value>The create default service locator function.</value>
        public static Func<IServiceLocator, ITypeFactory> CreateTypeFactoryFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createTypeFactoryFunc;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _createTypeFactoryFunc = value;
                }
            }
        }

        /// <summary>
        /// Called when an assembly gets loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssemblyLoadedEventArgs"/> instance containing the event data.</param>
        private static void OnAssemblyLoaded(object? sender, AssemblyLoadedEventArgs e)
        {
            lock (_lockObject)
            {
                _serviceLocatorInitializers = null;
            }
        }

        /// <summary>
        /// Creates a service locator with all the customized components.
        /// </summary>
        /// <param name="initializeServiceLocator">if set to <c>true</c>, the <see cref="IServiceLocator"/> will be initialized using the <see cref="IServiceLocatorInitializer"/> interface.</param>
        /// <returns>The newly created <see cref="IServiceLocator" />.</returns>
        public static IServiceLocator CreateServiceLocator(bool initializeServiceLocator = true)
        {
            lock (_lockObject)
            {
                var serviceLocator = CreateServiceLocatorFunc();
                if (serviceLocator is null)
                {
                    throw Log.ErrorAndCreateException<Exception>("Failed to create the IServiceLocator instance using the factory method");
                }

                if (!serviceLocator.IsTypeRegistered<IDependencyResolver>())
                {
                    var dependencyResolver = CreateDependencyResolverFunc(serviceLocator);
                    if (dependencyResolver is null)
                    {
                        throw Log.ErrorAndCreateException<Exception>("Failed to create the IDependencyResolver instance using the factory method");
                    }

                    serviceLocator.RegisterInstance(typeof(IDependencyResolver), dependencyResolver);
                }

                if (!serviceLocator.IsTypeRegistered<ITypeFactory>())
                {
                    var typeFactory = CreateTypeFactoryFunc(serviceLocator);
                    if (typeFactory is null)
                    {
                        throw Log.ErrorAndCreateException<Exception>("Failed to create the ITypeFactory instance using the factory method");
                    }

                    serviceLocator.RegisterInstance(typeof(ITypeFactory), typeFactory);
                }

                if (initializeServiceLocator)
                {
                    if (_serviceLocatorInitializers is null)
                    {
                        _serviceLocatorInitializers = new List<Type>(TypeCache.GetTypes(x => !x.IsInterfaceEx() & x.ImplementsInterfaceEx<IServiceLocatorInitializer>(), false));
                    }

                    foreach (var serviceLocatorInitializer in _serviceLocatorInitializers)
                    {
                        try
                        {
                            var initializer = Activator.CreateInstance(serviceLocatorInitializer) as IServiceLocatorInitializer;
                            if (initializer is not null)
                            {
                                initializer.Initialize(serviceLocator);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw Log.ErrorAndCreateException<Exception>(ex, "Failed to initialize service locator using initializer '{0}'", serviceLocatorInitializer.GetSafeFullName(false));
                        }
                    }
                }

                return serviceLocator;
            }
        }
    }
}
