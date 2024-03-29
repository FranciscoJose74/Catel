﻿namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Catel.Caching;

    /// <summary>
    /// Member info extensions.
    /// </summary>
    public static class MemberInfoExtensions
    {
        private static readonly ICacheStorage<ConstructorInfo, string> _constructorSignatureCache = new CacheStorage<ConstructorInfo, string>();
        private static readonly ICacheStorage<MethodInfo, string> _methodSignatureCache = new CacheStorage<MethodInfo, string>();

        /// <summary>
        /// Gets the signature of a method.
        /// </summary>
        /// <param name="constructorInfo">The member info.</param>
        /// <returns>The signature of the member info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="constructorInfo"/> is <c>null</c>.</exception>
        public static string GetSignature(this ConstructorInfo constructorInfo)
        {
            return _constructorSignatureCache.GetFromCacheOrFetch(constructorInfo, () =>
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(GetMethodBaseSignaturePrefix(constructorInfo));
                stringBuilder.Append("ctor(");
                var param = constructorInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}");
                stringBuilder.Append(string.Join(", ", param));
                stringBuilder.Append(")");

                return stringBuilder.ToString();
            });
        }

        /// <summary>
        /// Sort constructors by parameters match distance.
        /// </summary>
        /// <param name="constructors">The constructors</param>
        /// <param name="parameters">The constructor parameters</param>
        /// <returns>
        /// The constructors sorted by match distance.
        /// </returns>
        public static IEnumerable<ConstructorInfo> SortByParametersMatchDistance(this List<ConstructorInfo> constructors, object?[] parameters)
        {
            if (constructors.Count > 1)
            {
                var constructorDistances = new List<ConstructorDistance>();

                foreach (var constructor in constructors)
                {
                    if (constructor.TryGetConstructorDistanceByParametersMatch(parameters, out var distance))
                    {
                        constructorDistances.Add(new ConstructorDistance(distance, constructor));
                    }
                }

                return constructorDistances.OrderBy(constructor => constructor.Distance).Select(constructor => constructor.Constructor);
            }

            return constructors;
        }

        /// <summary>
        /// Try to get the constructor distance by parameters match.
        /// </summary>
        /// <param name="constructor">The constructor info</param>
        /// <param name="parameters"></param>
        /// <param name="distance">The distance</param>
        /// <returns><c>true</c> whether the constructor match with the parameters and distance can be computed; otherwise <c>false</c></returns>
        public static bool TryGetConstructorDistanceByParametersMatch(this ConstructorInfo constructor, object?[] parameters, out int distance)
        {
            distance = 0;
            var constructorParameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var constructorParameterType = constructorParameters[i].ParameterType;

                if (parameter is null && !constructorParameterType.IsClassEx() && !constructorParameterType.IsNullableType())
                {
                    return false;
                }

                if (parameter is not null && !constructorParameterType.IsAssignableFromEx(parameter.GetType()))
                {
                    return false;
                }

                if (parameter is not null)
                {
                    var parameterType = parameter.GetType();
                    int typeDistance = parameterType.GetTypeDistance(constructorParameterType);
                    if (typeDistance == -1)
                    {
                        return false;
                    }

                    distance += typeDistance * typeDistance;
                }
            }

            return true;
        }


        /// <summary>
        /// Gets the signature of a method.
        /// </summary>
        /// <param name="methodInfo">The member info.</param>
        /// <returns>The signature of the member info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methodInfo"/> is <c>null</c>.</exception>
        public static string GetSignature(this MethodInfo methodInfo)
        {
            return _methodSignatureCache.GetFromCacheOrFetch(methodInfo, () =>
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(GetMethodBaseSignaturePrefix(methodInfo));
                stringBuilder.Append(methodInfo.ReturnType.Name + " ");

                stringBuilder.Append(methodInfo.Name + "(");
                var param = methodInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}");
                stringBuilder.Append(string.Join(", ", param));
                stringBuilder.Append(")");

                return stringBuilder.ToString();
            });
        }

        private static string GetMethodBaseSignaturePrefix(MethodBase methodBase)
        {
            var stringBuilder = new StringBuilder();

            if (methodBase.IsPrivate)
            {
                stringBuilder.Append("private ");
            }
            else if (methodBase.IsPublic)
            {
                stringBuilder.Append("public ");
            }
            if (methodBase.IsAbstract)
            {
                stringBuilder.Append("abstract ");
            }
            if (methodBase.IsStatic)
            {
                stringBuilder.Append("static ");
            }
            if (methodBase.IsVirtual)
            {
                stringBuilder.Append("virtual ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns whether property is static.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            // Note: we decided to implement fastest path out, so as soon as we hit something non-static, exit
            if (propertyInfo.CanRead && !(propertyInfo.GetMethod?.IsStatic ?? false))
            {
                return false;
            }

            if (propertyInfo.CanWrite && !(propertyInfo.SetMethod?.IsStatic ?? false))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Constructor distance tuple.
        /// </summary>
        private class ConstructorDistance
        {
            public ConstructorDistance(int distance, ConstructorInfo constructor)
            {
                Distance = distance;
                Constructor = constructor;
            }

            /// <summary>
            /// Gets the distance.
            /// </summary>
            public int Distance { get; }

            /// <summary>
            /// Get the constructor.
            /// </summary>
            public ConstructorInfo Constructor { get; }
        }
    }
}
