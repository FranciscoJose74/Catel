﻿namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Logging;
    using Reflection;

    /// <summary>
    /// Container class for <see cref="ViewToViewModelMapping"/> elements.
    /// </summary>
    internal class ViewToViewModelMappingContainer
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing all the view to view model mappings.
        /// </summary>
        private readonly Dictionary<string, ViewToViewModelMapping> _viewToViewModelMappings = new Dictionary<string, ViewToViewModelMapping>();

        /// <summary>
        /// Dictionary containing all the view model to view mappings.
        /// </summary>
        private readonly Dictionary<string, ViewToViewModelMapping> _viewModelToViewMappings = new Dictionary<string, ViewToViewModelMapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewToViewModelMappingContainer" /> class.
        /// </summary>
        /// <param name="viewModelContainerType">The view model container type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelContainerType" /> is <c>null</c>.</exception>
        public ViewToViewModelMappingContainer(Type viewModelContainerType)
        {
            ArgumentNullException.ThrowIfNull(viewModelContainerType);

            var properties = viewModelContainerType.GetPropertiesEx();

            foreach (var property in properties)
            {
                var viewToViewModelAttributes = property.GetCustomAttributesEx(typeof(ViewToViewModelAttribute), false);
                if (viewToViewModelAttributes.Length > 0)
                {
                    Log.Debug("Property '{0}' is decorated with the ViewToViewModelAttribute, creating a mapping", property.Name);

                    var viewToViewModelAttribute = (ViewToViewModelAttribute)viewToViewModelAttributes[0];

                    var propertyName = property.Name;
                    var viewModelPropertyName = (string.IsNullOrEmpty(viewToViewModelAttribute.ViewModelPropertyName)) ? propertyName : viewToViewModelAttribute.ViewModelPropertyName;

                    var mapping = new ViewToViewModelMapping(propertyName, viewModelPropertyName, viewToViewModelAttribute.MappingType);

                    // Store it (in 2 dictionaries for high-speed access)
                    _viewToViewModelMappings.Add(property.Name, mapping);
                    _viewModelToViewMappings.Add(viewModelPropertyName, mapping);
                }
            }
        }

        /// <summary>
        /// Gets all the <see cref="ViewToViewModelMapping"/> that are registered.
        /// </summary>
        /// <returns><see cref="IEnumerable{ViewToViewModelMapping}"/> containing all registered <see cref="ViewToViewModelMapping"/>.</returns>
        public IEnumerable<ViewToViewModelMapping> GetAllViewToViewModelMappings()
        {
            return _viewToViewModelMappings.Select(mapping => mapping.Value);
        }

        /// <summary>
        /// Determines whether the manager contains a view to view model property mapping for the specified view property name.
        /// </summary>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <returns>
        /// <c>true</c> if the manager contains a view to view model property mapping for the specified view property name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsViewToViewModelMapping(string viewPropertyName)
        {
            return _viewToViewModelMappings.ContainsKey(viewPropertyName);
        }

        /// <summary>
        /// Gets the <see cref="ViewToViewModelMapping"/> that is mapped to the specified view property name.
        /// </summary>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <returns><see cref="ViewToViewModelMapping"/>.</returns>
        public ViewToViewModelMapping GetViewToViewModelMapping(string viewPropertyName)
        {
            return _viewToViewModelMappings[viewPropertyName];
        }

        /// <summary>
        /// Determines whether the manager contains a view model to view property mapping for the specified view model property name.
        /// </summary>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <returns>
        /// <c>true</c> if the manager contains a view model to view property mapping for the specified view model property name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsViewModelToViewMapping(string viewModelPropertyName)
        {
            return _viewModelToViewMappings.ContainsKey(viewModelPropertyName);
        }

        /// <summary>
        /// Gets the <see cref="ViewToViewModelMapping"/> that is mapped to the specified view model property name.
        /// </summary>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <returns><see cref="ViewToViewModelMapping"/>.</returns>
        public ViewToViewModelMapping GetViewModelToViewMapping(string viewModelPropertyName)
        {
            return _viewModelToViewMappings[viewModelPropertyName];
        }
    }
}
