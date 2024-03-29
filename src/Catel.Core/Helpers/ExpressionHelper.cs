﻿namespace Catel
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Logging;
    using Reflection;

    /// <summary>
    /// Expression helper class that allows easy parsing of expressions.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the name of the property from the expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The name of the property parsed from the expression or <c>null</c> if the property cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyExpression)
        {
            ArgumentNullException.ThrowIfNull(propertyExpression);

            return PropertyHelper.GetPropertyName(propertyExpression);
        }

        /// <summary>
        /// Gets the name of the property from the expression.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>
        /// The name of the property parsed from the expression or <c>null</c> if the property cannot be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            ArgumentNullException.ThrowIfNull(propertyExpression);

            return PropertyHelper.GetPropertyName(propertyExpression);
        }

        /// <summary>
        /// Gets the owner of the expression. For example if the expression <c>() => MyProperty</c>, the owner of the
        /// property will be returned.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The owner of the expression or <c>null</c> if the owner cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        public static object? GetOwner<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            ArgumentNullException.ThrowIfNull(propertyExpression);

            var expressionToHandle = GetExpressionToHandle(propertyExpression);

            var body = expressionToHandle as MemberExpression;
            if (body is null)
            {
                Log.Warning("Failed to retrieve the body of the expression (value is null)");
                return null;
            }

            var constantExpression = body.Expression as ConstantExpression;
            if (constantExpression is not null)
            {
                return constantExpression.Value;
            }

            var memberExpression = body.Expression as MemberExpression;
            if (memberExpression is not null)
            {
                var resolvedMemberExpression = ResolveMemberExpression(memberExpression);
                return resolvedMemberExpression;
            }

            return null;
        }

        private static object? ResolveMemberExpression(MemberExpression memberExpression)
        {
            var fieldInfo = memberExpression.Member as FieldInfo;
            if (fieldInfo is not null)
            {
                var ownerConstantExpression = memberExpression.Expression as ConstantExpression;
                if (ownerConstantExpression is not null)
                {
                    return fieldInfo.GetValue(ownerConstantExpression.Value);
                }
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo is not null)
            {
                var ownerConstantExpression = memberExpression.Expression as ConstantExpression;
                if (ownerConstantExpression is not null)
                {
                    return propertyInfo.GetValue(ownerConstantExpression.Value, null);
                }

                // Note: this is support for .NET native
                var subMemberExpression = memberExpression.Expression as MemberExpression;
                if (subMemberExpression is not null)
                {
                    var resolvedMemberExpression = ResolveMemberExpression(subMemberExpression);
                    return propertyInfo.GetValue(resolvedMemberExpression, null);
                }
            }

            // Fallback but is a bit slower
            var lamdaExpression = Expression.Lambda(memberExpression);
            return lamdaExpression.Compile().DynamicInvoke();
        }

        private static Expression GetExpressionToHandle<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var expressionToHandle = propertyExpression.Body; 

            // Might occur in Android, maybe on other platforms as well
            var unaryExpression = expressionToHandle as UnaryExpression;
            if (unaryExpression is not null)
            {
                expressionToHandle = unaryExpression.Operand;
            }

            return expressionToHandle;
        }
    }
}
