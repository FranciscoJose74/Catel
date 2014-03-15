﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MvcModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.IoC;

    /// <summary>
    /// Mvc module which allows the registration of default services in the service locator.
    /// </summary>
    public class MvcModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            // Register services here
        }
    }
}