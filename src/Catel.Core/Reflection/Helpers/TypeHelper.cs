﻿namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Logging;

    /// <summary>
    /// <see cref="Type"/> helper class.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        ///   The <see cref = "ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A list of microsoft public key tokens.
        /// </summary>
        private static readonly HashSet<string> _microsoftPublicKeyTokens;

        private const char InnerTypeCountStart = '`';
        private const char InternalTypeStart = '+';
        private const char InternalTypeEnd = '[';
        private const string AllTypesStart = "[[";
        private const char SingleTypeStart = '[';
        private const char SingleTypeEnd = ']';
        private static readonly char[] InnerTypeCountEnd = new[] { '[', '+' };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        static TypeHelper()
        {
            _microsoftPublicKeyTokens = new HashSet<string>
            {
                "b77a5c561934e089",
                "b03f5f7f11d50a3a",
                "31bf3856ad364e35"
            };
        }

        /// <summary>
        /// Gets the Microsoft public key tokens.
        /// </summary>
        /// <value>The Microsoft public key tokens.</value>
        public static IEnumerable<string> MicrosoftPublicKeyTokens
        {
            get { return _microsoftPublicKeyTokens; }
        }

        /// <summary>
        /// Gets the typed instance based on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to retrieve in the typed form.</param>
        /// <returns>The typed instance.</returns>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public static TTargetType? GetTypedInstance<TTargetType>(object instance)
            where TTargetType : class
        {
            if (instance is null)
            {
                return null;
            }

            var typedInstance = instance as TTargetType;
            if (typedInstance is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Expected an instance of '{0}', but retrieved an instance of '{1}', cannot return the typed instance", typeof(TTargetType).Name, instance.GetType().Name);
            }

            return typedInstance;
        }

        /// <summary>
        ///   Determines whether the subclass is of a raw generic type.
        /// </summary>
        /// <param name = "generic">The generic.</param>
        /// <param name = "toCheck">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the subclass is of a raw generic type; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This implementation is based on this forum thread:
        /// http://stackoverflow.com/questions/457676/c-reflection-check-if-a-class-is-derived-from-a-generic-class
        /// <para />
        /// Original license: CC BY-SA 2.5, compatible with the MIT license.
        /// </remarks>
        /// <exception cref = "ArgumentNullException">The <paramref name = "generic" /> is <c>null</c>.</exception>
        /// <exception cref = "ArgumentNullException">The <paramref name = "toCheck" /> is <c>null</c>.</exception>
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            ArgumentNullException.ThrowIfNull(generic);
            ArgumentNullException.ThrowIfNull(toCheck);

            var processedType = toCheck;

            while ((processedType is not null) && (processedType != typeof(object)))
            {
                var cur = processedType.IsGenericTypeEx() ? processedType.GetGenericTypeDefinition() : processedType;
                if (generic == cur)
                {
                    return true;
                }

                processedType = processedType.GetBaseTypeEx();
            }

            return false;
        }

        /// <summary>
        /// Gets the assembly name without overhead (version, public keytoken, etc)
        /// </summary>
        /// <param name="fullyQualifiedAssemblyName">Name of the fully qualified assembly.</param>
        /// <returns>The assembly without the overhead.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullyQualifiedAssemblyName"/> is <c>null</c> or whitespace.</exception>
        public static string GetAssemblyNameWithoutOverhead(string fullyQualifiedAssemblyName)
        {
            Argument.IsNotNullOrWhitespace("fullyQualifiedAssemblyName", fullyQualifiedAssemblyName);

            var indexOfFirstComma = fullyQualifiedAssemblyName.IndexOf(',');
            if (indexOfFirstComma != -1)
            {
                return fullyQualifiedAssemblyName.Substring(0, indexOfFirstComma);
            }

            return fullyQualifiedAssemblyName;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
        /// <returns>The assembly name retrieved from the type, for example <c>Catel.Core</c> or <c>null</c> if the assembly is not contained by the type.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
        public static string? GetAssemblyName(string fullTypeName)
        {
            Argument.IsNotNullOrWhitespace("fullTypeName", fullTypeName);

            var lastGenericIndex = fullTypeName.LastIndexOf("]]", StringComparison.Ordinal);
            if (lastGenericIndex != -1)
            {
                fullTypeName = fullTypeName.Substring(lastGenericIndex + 2);
            }

            var splitterPos = fullTypeName.IndexOf(", ", StringComparison.Ordinal);
            var assemblyName = (splitterPos != -1) ? fullTypeName.Substring(splitterPos + 1).Trim() : null;
            return assemblyName;
        }

        /// <summary>
        /// Gets the type name with assembly, but without the fully qualified assembly name. For example, this method provides
        /// the string:
        /// <para />
        /// <c>Catel.TypeHelper, Catel.Core, Version=1.0.0.0, PublicKeyToken=123456789</c>
        /// <para />
        /// and will return:
        /// <para />
        /// <c>Catel.TypeHelper, Catel.Core</c>
        /// </summary>
        /// <param name="fullTypeName">Full name of the type.</param>
        /// <returns>The type name including the assembly.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
        public static string GetTypeNameWithAssembly(string fullTypeName)
        {
            Argument.IsNotNullOrWhitespace("fullTypeName", fullTypeName);

            var assemblyNameWithoutOverhead = GetAssemblyName(fullTypeName);
            if (assemblyNameWithoutOverhead is null)
            {
                return fullTypeName;
            }

            var assemblyName = GetAssemblyNameWithoutOverhead(assemblyNameWithoutOverhead);
            var typeName = GetTypeName(fullTypeName);

            return FormatType(assemblyName, typeName);
        }

        /// <summary>
        /// Gets the name of the type without the assembly but including the namespace.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
        /// <returns>The type name retrieved from the type, for example <c>Catel.TypeHelper</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
        public static string GetTypeName(string fullTypeName)
        {
            Argument.IsNotNullOrWhitespace("fullTypeName", fullTypeName);

            return ConvertTypeToVersionIndependentType(fullTypeName, true);
        }

        /// <summary>
        /// Gets the type name without the assembly namespace.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
        /// <returns>The type name retrieved from the type, for example <c>TypeHelper</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
        public static string GetTypeNameWithoutNamespace(string fullTypeName)
        {
            Argument.IsNotNullOrWhitespace("fullTypeName", fullTypeName);

            fullTypeName = GetTypeName(fullTypeName);

            var splitterPos = fullTypeName.LastIndexOf(".", StringComparison.Ordinal);

            var typeName = (splitterPos != -1) ? fullTypeName.Substring(splitterPos + 1).Trim() : fullTypeName;
            return typeName;
        }

        /// <summary>
        /// Gets the type namespace.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
        /// <returns>The type namespace retrieved from the type, for example <c>Catel</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
        public static string GetTypeNamespace(string fullTypeName)
        {
            Argument.IsNotNullOrWhitespace("fullTypeName", fullTypeName);

            fullTypeName = GetTypeName(fullTypeName);

            var splitterPos = fullTypeName.LastIndexOf(".", StringComparison.Ordinal);

            var typeName = (splitterPos != -1) ? fullTypeName.Substring(0, splitterPos).Trim() : fullTypeName;
            return typeName;
        }

        /// <summary>
        ///   Formats a type in the official type description like [typename], [assemblyname].
        /// </summary>
        /// <param name = "assembly">Assembly name to format.</param>
        /// <param name = "type">Type name to format.</param>
        /// <returns>Type name like [typename], [assemblyname].</returns>
        /// <exception cref="ArgumentException">The <paramref name="assembly"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="type"/> is <c>null</c> or whitespace.</exception>
        public static string FormatType(string assembly, string type)
        {
            Argument.IsNotNullOrWhitespace("assembly", assembly);
            Argument.IsNotNullOrWhitespace("type", type);

            return $"{type}, {assembly}";
        }

        /// <summary>
        /// Formats multiple inner types into one string.
        /// </summary>
        /// <param name="innerTypes">The inner types.</param>
        /// <param name="stripAssemblies">if set to <c>true</c>, the assembly names will be stripped as well.</param>
        /// <returns>string representing a combination of all inner types.</returns>
        public static string FormatInnerTypes(IEnumerable<string> innerTypes, bool stripAssemblies = false)
        {
            return string.Join(",", innerTypes.Select(x =>
            {
                var type = stripAssemblies ? ConvertTypeToVersionIndependentType(x, true) : x;
                return $"[{type}]";
            }));
        }

        /// <summary>
        /// Converts a string representation of a type to a version independent type by removing the assembly version information.
        /// </summary>
        /// <param name="type">Type to convert.</param>
        /// <param name="stripAssemblies">if set to <c>true</c>, the assembly names will be stripped as well.</param>
        /// <returns>String representing the type without version information.</returns>
        /// <exception cref="ArgumentException">The <paramref name="type" /> is <c>null</c> or whitespace.</exception>
        public static string ConvertTypeToVersionIndependentType(string type, bool stripAssemblies = false)
        {
            Argument.IsNotNullOrWhitespace("type", type);

            const string innerTypesEnd = ",";

            var newType = type;
            var innerTypes = GetInnerTypes(newType);

            if (innerTypes.Length > 0)
            {
                // Remove inner types, but never strip assemblies because we need the real original type
                newType = newType.Replace($"[{FormatInnerTypes(innerTypes, false)}]", string.Empty);
                for (var i = 0; i < innerTypes.Length; i++)
                {
                    innerTypes[i] = ConvertTypeToVersionIndependentType(innerTypes[i], stripAssemblies);
                }
            }

            var splitterPos = newType.IndexOf(", ", StringComparison.Ordinal);
            var typeName = (splitterPos != -1) ? newType.Substring(0, splitterPos).Trim() : newType;
            var assemblyName = GetAssemblyName(newType);

            // Remove version info from assembly (if not signed by Microsoft)
            if (!string.IsNullOrWhiteSpace(assemblyName) && !stripAssemblies)
            {
                var isMicrosoftAssembly = MicrosoftPublicKeyTokens.Any(t => assemblyName.Contains(t));
                if (!isMicrosoftAssembly)
                {
                    assemblyName = GetAssemblyNameWithoutOverhead(assemblyName);
                }

                newType = FormatType(assemblyName, typeName);
            }
            else
            {
                newType = typeName;
            }

            if (innerTypes.Length > 0)
            {
                var innerTypesIndex = stripAssemblies ? newType.Length : newType.IndexOf(innerTypesEnd);
                if (innerTypesIndex >= 0)
                {
                    newType = newType.Insert(innerTypesIndex, $"[{FormatInnerTypes(innerTypes, stripAssemblies)}]");
                }
            }

            return newType;
        }

        /// <summary>
        /// Returns the inner type of a type, for example, a generic array type.
        /// </summary>
        /// <param name="type">Full type which might contain an inner type.</param>
        /// <returns>Array of inner types.</returns>
        /// <exception cref="ArgumentException">The <paramref name="type"/> is <c>null</c> or whitespace.</exception>
        public static string[] GetInnerTypes(string type)
        {
            Argument.IsNotNullOrWhitespace("type", type);

            var innerTypes = new List<string>();

            try
            {
                var countIndex = type.IndexOf(InnerTypeCountStart);
                if (countIndex == -1)
                {
                    return innerTypes.ToArray();
                }

                // This is a generic, but does the type definition also contain the inner types?
                if (!type.Contains(AllTypesStart))
                {
                    return innerTypes.ToArray();
                }

                // Get the number of inner types
                var innerTypeCountEnd = -1;
                foreach (var t in InnerTypeCountEnd)
                {
                    var index = type.IndexOf(t);
                    if ((index != -1) && ((innerTypeCountEnd == -1) || (index < innerTypeCountEnd)))
                    {
                        // This value is more likely to be the one
                        innerTypeCountEnd = index;
                    }
                }

                var innerTypeCount = int.Parse(type.Substring(countIndex + 1, innerTypeCountEnd - countIndex - 1));

                // Remove all info until the first inner type
                if (!type.Contains(InternalTypeStart.ToString()))
                {
                    // Just remove the info
                    type = type.Substring(innerTypeCountEnd + 1);
                }
                else
                {
                    // Remove the index, but not the numbers
                    var internalTypeEnd = type.IndexOf(InternalTypeEnd);
                    type = type.Substring(internalTypeEnd + 1);
                }

                // Get all the inner types
                for (var i = 0; i < innerTypeCount; i++)
                {
                    // Get the start & end of this inner type
                    var innerTypeStart = type.IndexOf(SingleTypeStart);
                    var innerTypeEnd = innerTypeStart + 1;
                    var openings = 1;

                    // Loop until we find the end
                    while (openings > 0)
                    {
                        if (type[innerTypeEnd] == SingleTypeStart)
                        {
                            openings++;
                        }
                        else if (type[innerTypeEnd] == SingleTypeEnd)
                        {
                            openings--;
                        }

                        // Increase current pos if we still have openings left
                        if (openings > 0)
                        {
                            innerTypeEnd++;
                        }
                    }

                    innerTypes.Add(type.Substring(innerTypeStart + 1, innerTypeEnd - innerTypeStart - 1));
                    type = type.Substring(innerTypeEnd + 1);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to retrieve inner types");
            }

            return innerTypes.ToArray();
        }

        /// <summary>
        ///   Tries to Generic cast of a value.
        /// </summary>
        /// <typeparam name = "TOutput">Requested return type.</typeparam>
        /// <typeparam name = "TInput">The input type.</typeparam>
        /// <param name = "value">The value to cast.</param>
        /// <param name = "output">The casted value.</param>
        /// <returns>When a cast is succeded true else false.</returns>
        public static bool TryCast<TOutput, TInput>(TInput? value, out TOutput output)
        {
            var success = true;

            try
            {
                var outputType = typeof(TOutput);
                var innerType = Nullable.GetUnderlyingType(outputType);

                // Database support...
                if (value is null)
                {
                    output = default!;

                    if (outputType.IsValueTypeEx() && innerType is null)
                    {
                        success = false;
                    }
                    else
                    {
                        // Non-valuetype can contain nill.
                        // (Nullable<T> also)
                    }
                }
                else
                {
                    var inputType = value.GetType();
                    if (inputType.IsAssignableFromEx(outputType))
                    {
                        // Direct assignable
                        output = (TOutput)(object)value;
                    }
                    else
                    {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                        output = (TOutput)Convert.ChangeType(value, innerType ?? outputType, CultureInfo.InvariantCulture);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                    }
                }
            }
            catch (Exception)
            {
                output = default!;
                success = false;
            }

            return success;
        }

        /// <summary>
        ///   Generic cast of a value.
        /// </summary>
        /// <typeparam name = "TOutput">Requested return type.</typeparam>
        /// <typeparam name = "TInput">The input type.</typeparam>
        /// <param name = "value">The value to cast.</param>
        /// <returns>The casted value.</returns>
        public static TOutput Cast<TOutput, TInput>(TInput? value)
        {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return Cast<TOutput>(value);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

        /// <summary>
        ///   Generic cast of a value.
        /// </summary>
        /// <typeparam name = "TOutput">Requested return type.</typeparam>
        /// <param name = "value">The value to cast.</param>
        /// <returns>The casted value.</returns>
        public static TOutput Cast<TOutput>(object? value)
        {
            if (!TryCast(value, out TOutput output))
            {
                var tI = (value ?? typeof(object)).GetType().GetSafeFullName(false);
                var tO = typeof(TOutput).FullName ?? typeof(TOutput).Name;
                var vl = string.Concat(value);
                var msg = "Failed to cast from '{0}' to '{1}'";

                if (!tI.Equals(vl))
                {
                    msg = string.Concat(msg, " for value '{2}'");
                }

                throw Log.ErrorAndCreateException<InvalidCastException>(msg, tI, tO, vl);
            }

            return output;
        }

        /// <summary>
        ///   Generic cast of a value.
        /// </summary>
        /// <typeparam name = "TOutput">Requested return type.</typeparam>
        /// <typeparam name = "TInput">The input type.</typeparam>
        /// <param name = "value">The value to cast.</param>
        /// <param name = "whenNullValue">When unable to cast the incoming value, this value is returned instead.</param>
        /// <returns>The casted value or when uncastable the <paramref name = "whenNullValue" /> is returned.</returns>
        public static TOutput Cast<TOutput, TInput>(TInput? value, TOutput whenNullValue)
        {
            if (!TryCast(value, out TOutput output) || output is null)
            {
                output = whenNullValue;
            }

            return output;
        }
    }
}
