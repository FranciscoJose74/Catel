﻿namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Logging;

    /// <summary>
    /// A value converter which contains a list of IValueConverters and invokes their Convert or ConvertBack methods
    /// in the order that they exist in the list.  The output of one converter is piped into the next converter
    /// allowing for modular value converters to be chained together.  If the ConvertBack method is invoked, the
    /// value converters are executed in reverse order (highest to lowest index).  Do not leave an element in the
    /// Converters property collection null, every element must reference a valid IValueConverter instance. If a
    /// value converter's type is not decorated with the ValueConversionAttribute, an InvalidOperationException will be
    /// thrown when the converter is added to the Converters collection.
    /// <para />
    /// Known limitations: the supplied parameter is used for ALL! converters.
    /// </summary>
    /// <remarks>
    /// Original source: http://www.codeproject.com/KB/WPF/PipingValueConverters_WPF.aspx
    /// <para />
    /// Original license: CPOL, compatible with the MIT license.
    /// </remarks>
    [ContentProperty(nameof(Converters))]
    public class ValueConverterGroup : IValueConverter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ObservableCollection<System.Windows.Data.IValueConverter> _converters = new ObservableCollection<System.Windows.Data.IValueConverter>();
        private readonly Dictionary<System.Windows.Data.IValueConverter, ValueConversionAttribute?> _cachedAttributes = new Dictionary<System.Windows.Data.IValueConverter, ValueConversionAttribute?>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterGroup"/> class.
        /// </summary>
        public ValueConverterGroup()
        {
            _converters.CollectionChanged += OnConvertersCollectionChanged;
        }

        /// <summary>
        /// Returns the list of IValueConverters contained in this converter.
        /// </summary>
        public ObservableCollection<System.Windows.Data.IValueConverter> Converters
        {
            get { return _converters; }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            object? output = value;
            
            for (var i = 0; i < Converters.Count; ++i)
            {
                var converter = Converters[i];
                var currentTargetType = GetTargetType(i, targetType, true);
                output = converter.Convert(output, currentTargetType, parameter, culture);

                // If the converter returns 'DoNothing' then the binding operation should terminate.
                if (output == Binding.DoNothing)
                {
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            object? output = value;

            for (int i = Converters.Count - 1; i > -1; --i)
            {
                var converter = Converters[i];
                var currentTargetType = GetTargetType(i, targetType, false);
                output = converter.ConvertBack(output, currentTargetType, parameter, culture);

                // When a converter returns 'DoNothing' the binding operation should terminate.
                if (output == Binding.DoNothing)
                {
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Returns the target type for a conversion operation.
        /// </summary>
        /// <param name="converterIndex">The index of the current converter about to be executed.</param>
        /// <param name="finalTargetType">The 'targetType' argument passed into the conversion method.</param>
        /// <param name="convert">Pass true if calling from the Convert method, or false if calling from ConvertBack.</param>
        protected virtual Type GetTargetType(int converterIndex, Type finalTargetType, bool convert)
        {
            // If the current converter is not the last/first in the list, 
            // get a reference to the next/previous converter.
            System.Windows.Data.IValueConverter? nextConverter = null;
            if (convert)
            {
                if (converterIndex < Converters.Count - 1)
                {
                    nextConverter = Converters[converterIndex + 1];
                    if (nextConverter is null)
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>($"The Converters collection of the ValueConverterGroup contains a null reference at index: {(converterIndex + 1).ToString()}");
                    }
                }
            }
            else
            {
                if (converterIndex > 0)
                {
                    nextConverter = Converters[converterIndex - 1];
                    if (nextConverter is null)
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>($"The Converters collection of the ValueConverterGroup contains a null reference at index: {(converterIndex - 1).ToString()}");
                    }
                }
            }

            if (nextConverter is not null)
            {
                var conversionAttribute = _cachedAttributes[nextConverter];
                if (conversionAttribute is not null)
                {
                    // If the Convert method is going to be called, we need to use the SourceType of the next 
                    // converter in the list.  If ConvertBack is called, use the TargetType.
                    return convert ? conversionAttribute.SourceType : conversionAttribute.TargetType;
                }
            }

            // If the current converter is the last one to be executed return the target type passed into the conversion method.
            return finalTargetType;
        }

        /// <summary>
        /// Called when [converters collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnConvertersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // The 'Converters' collection has been modified, so validate that each value converter it now
            // contains is decorated with ValueConversionAttribute and then cache the attribute value.

            IList? convertersToProcess = null;
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                convertersToProcess = e.NewItems;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var oldItems = e.OldItems;
                if (oldItems is not null)
                {
                    foreach (IValueConverter converter in oldItems)
                    {
                        _cachedAttributes.Remove(converter);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _cachedAttributes.Clear();
                convertersToProcess = _converters;
            }

            if (convertersToProcess is not null && convertersToProcess.Count > 0)
            {
                foreach (IValueConverter converter in convertersToProcess)
                {
                    object[] attributes = converter.GetType().GetCustomAttributes(typeof (ValueConversionAttribute), false);

                    // Maybe it is 'beter' (more robust) to use a default ValueConversion(typeof(object), typeof(bool)) attribute.
                    if (attributes.Length != 1)
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>("All value converters added to a ValueConverterGroup must be decorated with the ValueConversionAttribute attribute exactly once.");
                    }

                    _cachedAttributes.Add(converter, attributes[0] as ValueConversionAttribute);
                }
            }
        }
    }
}
