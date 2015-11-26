﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectToDisplayNameConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ComponentModel;
    using Converters;
    using Reflection;
    using Services;

    /// <summary>
    /// Converts elements to a display name. This supports classes, member info, enums, etc.
    /// </summary>
    public class ObjectToDisplayNameConverter : ValueConverterBase
    {
        /// <summary>
        /// Gets or sets the language service. If this value is set, it will be used inside the <see cref="DisplayNameAttribute"/>.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; set; }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>System.Object.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var type = value as Type;
            if (type != null)
            {
                DisplayNameAttribute displayAttribute = null;
                if (type.TryGetAttribute(out displayAttribute))
                {
                    return GetDisplayName(displayAttribute);
                }

                return type.Name;
            }

            var memberInfo = value as MemberInfo;
            if (memberInfo != null)
            {
                DisplayNameAttribute displayAttribute = null;
                if (memberInfo.TryGetAttribute(out displayAttribute))
                {
                    return GetDisplayName(displayAttribute);
                }

                return memberInfo.Name;
            }

            // Support enum values
            if (value != null)
            {
                var valueType = value.GetType();
                if (valueType.IsEnumEx())
                {
                    memberInfo = valueType.GetMemberEx(value.ToString(), allowStaticMembers: true).FirstOrDefault();
                    if (memberInfo != null)
                    {
                        DisplayNameAttribute displayAttribute = null;
                        if (memberInfo.TryGetAttribute(out displayAttribute))
                        {
                            return GetDisplayName(displayAttribute);
                        }

                        return memberInfo.Name;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the display name from the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>System.String.</returns>
        protected string GetDisplayName(DisplayNameAttribute attribute)
        {
            var languageService = LanguageService;
            if (languageService != null)
            {
                attribute.LanguageService = languageService;
            }

            return attribute.DisplayName;
        }
    }
}