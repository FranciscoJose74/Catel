﻿namespace Catel.MVVM.Converters
{
    using System;
    using System.Windows;

    /// <summary>
    /// Convert from string to <see cref="Visibility"/>. 
    /// If the string is not null or empty, Visibility.Visible will be returned. 
    /// If the string is null or empty, Visibility.Collapsed will be returned.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(string), typeof(Visibility))]
    public class EmptyStringToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToCollapsingVisibilityConverter"/> class.
        /// </summary>
        public EmptyStringToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToCollapsingVisibilityConverter"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        internal EmptyStringToCollapsingVisibilityConverter(Visibility notVisibleVisibility)
            : base(notVisibleVisibility)
        {
        }

        /// <summary>
        /// Determines what value this converter should return.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is visible; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsVisible(object? value, Type targetType, object? parameter)
        {
            var stringValue = value as string;

            var isVisible = !string.IsNullOrEmpty(stringValue);

            // Note: base class will invert if needed

            return isVisible;
        }
    }

    /// <summary>
    /// Convert from string to <see cref="System.Windows.Visibility"/>. 
    /// If the string is not null or empty, Visibility.Visible will be returned. 
    /// If the string is null or empty, Visibility.Hidden will be returned.
    /// </summary>
    [System.Windows.Data.ValueConversion(typeof(string), typeof(Visibility))]
    public class EmptyStringToHidingVisibilityConverter : EmptyStringToCollapsingVisibilityConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringToHidingVisibilityConverter"/> class.
        /// </summary>
        public EmptyStringToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
        }
    }
}
