﻿namespace Catel.Reflection
{
    using System;
    using System.Linq;
    using Collections;

    /// <summary>
    /// Object extensions for reflection.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts the list of objects to an array of attributes, very easy to use during GetCustomAttribute reflection.
        /// </summary>
        /// <param name="objects">The object array, can be <c>null</c>.</param>
        /// <returns>Attribute array or empty array if <paramref name="objects"/> is <c>null</c>.</returns>
        public static Attribute[] ToAttributeArray(this object[] objects)
        {
            if (objects is null)
            {
                return Array.Empty<Attribute>();
            }

            return objects.Cast<Attribute>().ToArray();
        }
    }
}
