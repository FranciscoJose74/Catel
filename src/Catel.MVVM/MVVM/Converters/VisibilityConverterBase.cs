﻿namespace Catel.MVVM.Converters
{
    using System;
    using Catel.Data;
    using System.Windows;

    /// <summary>
    /// A base class that makes it easier to create values to visibility converters.
    /// </summary>
    public abstract class VisibilityConverterBase : ValueConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibilityConverterBase"/> class.
        /// </summary>
        /// <param name="notVisibleVisibility">The <see cref="Visibility"/> state when not visibible should be returned.</param>
        /// <exception cref="ArgumentException">The <paramref name="notVisibleVisibility"/> is <see cref="Visibility.Visible"/>.</exception>
        protected VisibilityConverterBase(Visibility notVisibleVisibility)
        {
            if (notVisibleVisibility == Visibility.Visible)
            {
                throw new ArgumentException(Catel.ResourceHelper.GetString("VisibilityIsNotAllowedForConverter"), "notVisibleVisibility");
            }

            NotVisibleVisibility = notVisibleVisibility;
        }

        /// <summary>
        /// Gets the <see cref="Visibility"/> state when not visibible should be returned.
        /// </summary>
        /// <value>The not visible visibility.</value>
        public Visibility NotVisibleVisibility { get; private set; }
        
        /// <summary>
        /// Determines what value this converter should return.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <returns><c>true</c> if the specified value is visible; otherwise, <c>false</c>.</returns>
        protected abstract bool IsVisible(object? value, Type targetType, object? parameter);

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object? Convert(object? value, Type targetType, object? parameter)
        {
            var isVisible = IsVisible(value, targetType, parameter);

            if (SupportInversionUsingCommandParameter && ConverterHelper.ShouldInvert(parameter))
            {
                isVisible = !isVisible;
            }

            return BoxingCache.GetBoxedValue(isVisible ? Visibility.Visible : NotVisibleVisibility);
        }
    }

    /// <summary>
    /// A base class that makes it easier to create values to visibility converters.
    /// <para />
    /// This converter returns <see cref="Visibility.Collapsed"/> when a non-visible state should be returned.
    /// </summary>
    public abstract class CollapsingVisibilityConverterBase : VisibilityConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsingVisibilityConverterBase"/> class.
        /// </summary>
        protected CollapsingVisibilityConverterBase()
            : base(Visibility.Collapsed)
        {
        }
    }

    /// <summary>
    /// A base class that makes it easier to create values to visibility converters.
    /// <para />
    /// This converter returns <see cref="Visibility.Hidden"/> when a non-visible state should be returned.
    /// </summary>
    public abstract class HidingVisibilityConverterBase : VisibilityConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HidingVisibilityConverterBase"/> class.
        /// </summary>
        protected HidingVisibilityConverterBase()
            : base(Visibility.Hidden)
        {
        }
    }
}
