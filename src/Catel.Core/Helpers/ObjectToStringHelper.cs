﻿namespace Catel
{
    using System;
    using System.Globalization;
    using Reflection;

    /// <summary>
    /// Simple static class that turns an object to string.
    /// </summary>
    public static class ObjectToStringHelper
    {
        /// <summary>
        /// Initializes static members of the <see cref="StringToObjectHelper"/> class.
        /// </summary>
        static ObjectToStringHelper()
        {
            DefaultCulture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the default culture to use for parsing.
        /// </summary>
        /// <value>The default culture.</value>
        public static CultureInfo? DefaultCulture { get; set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance, can be <c>null</c>.</param>
        /// <returns>A <see cref="string"/> that represents the instance.</returns>
        public static string ToString(object? instance)
        {
            return ToString(instance, DefaultCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the instance.
        /// <para />
        /// If the <paramref name="instance" /> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance, can be <c>null</c>.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>A <see cref="string" /> that represents the instance.</returns>
        public static string ToString(object? instance, CultureInfo? cultureInfo)
        {
            if (instance is null)
            {
                return "null";
            }

            if (instance == DBNull.Value)
            {
                return "dbnull";
            }

            var instanceType = instance.GetType();
            if (instanceType == typeof (DateTime) || instanceType == typeof (DateTime?))
            {
                return ((DateTime) instance).ToString(cultureInfo);
            }

            // Note: Not supported on NETFX_CORE, don't enable, really doesn't work. If you need a ToString
            // for a specific string, use a cast like the DateTime about

            // Check if there is a culture specific version
            var toStringMethod = instanceType.GetMethodEx("ToString", TypeArray.From<IFormatProvider>());
            if (toStringMethod is not null)
            {
                var toStringResult = (string?)toStringMethod.Invoke(instance, new object?[] { cultureInfo });
                if (toStringResult is not null)
                {
                    return toStringResult;
                }
            }

            return instance.ToString() ?? "null";
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the type name of the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="string"/> that represents the type of the instance.</returns>
        public static string ToTypeString(object? instance)
        {
            if (instance is null)
            {
                return "null";
            }

            var instanceAsType = instance as Type;
            if (instanceAsType is not null)
            {
                return instanceAsType.Name;
            }

            return instance.GetType().Name;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the full type name of the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="string"/> that represents the type of the instance.</returns>
        public static string ToFullTypeString(object? instance)
        {
            if (instance is null)
            {
                return "null";
            }

            var type = instance as Type;
            if (type is null)
            {
                type = instance.GetType();
            }

            return type.GetSafeFullName(false);
        }
    }
}
