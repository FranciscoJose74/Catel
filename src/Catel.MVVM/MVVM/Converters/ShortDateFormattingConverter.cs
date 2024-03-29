﻿namespace Catel.MVVM.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// ShortDateFormattingConverter
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(DateTime), typeof(string))]
    public class ShortDateFormattingConverter : FormattingConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortDateFormattingConverter"/> class.
        /// </summary>
        public ShortDateFormattingConverter()
            : base("d")
        {
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <remarks>
        /// By default, this method returns <see cref="ConverterHelper.UnsetValue"/>. This method only has
        /// to be overridden when it is actually used.
        /// </remarks>
        protected override object? ConvertBack(object? value, Type targetType, object? parameter)
        {
            var parsed = DateTime.TryParse(value as string, CurrentCulture, DateTimeStyles.None, out var dateTimeValue);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return parsed ? dateTimeValue : ConverterHelper.UnsetValue;
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }
    }
}
