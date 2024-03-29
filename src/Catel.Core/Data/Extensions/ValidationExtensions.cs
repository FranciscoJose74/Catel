﻿namespace Catel.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for validation.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Gets the validation summary for the specified <see cref="IValidationContext"/>.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The <see cref="IValidationSummary"/>.</returns>
        public static IValidationSummary GetValidationSummary(this IValidationContext validationContext, object? tag = null)
        {
            return new ValidationSummary(validationContext, tag);
        }

        /// <summary>
        /// Synchronizes the current with the specified context. This means that the current contains will become the same as the
        /// specified context.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <param name="additionalValidationContext">The additional validation context.</param>
        /// <param name="onlyAddValidation">if set to <c>true</c>, validation is only added, not removed. This is great to build up summaries.</param>
        /// <returns>The list of changes.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext" /> is <c>null</c>.</exception>
        public static List<ValidationContextChange> SynchronizeWithContext(this ValidationContext validationContext, IValidationContext additionalValidationContext, bool onlyAddValidation = false)
        {
            ArgumentNullException.ThrowIfNull(validationContext);
            ArgumentNullException.ThrowIfNull(additionalValidationContext);

            var changes = ValidationContextHelper.GetChanges(validationContext, additionalValidationContext);

            foreach (var change in changes)
            {
                var validationResultAsField = change.ValidationResult as IFieldValidationResult;
                var validationResultAsBusinessRule = change.ValidationResult as IBusinessRuleValidationResult;

                switch (change.ChangeType)
                {
                    case ValidationContextChangeType.Added:
                        if (validationResultAsField is not null)
                        {
                            validationContext.Add(validationResultAsField);
                        }
                        else if (validationResultAsBusinessRule is not null)
                        {
                            validationContext.Add(validationResultAsBusinessRule);
                        }
                        break;

                    case ValidationContextChangeType.Removed:
                        if (!onlyAddValidation)
                        {
                            if (validationResultAsField is not null)
                            {
                                validationContext.Remove(validationResultAsField);
                            }
                            else if (validationResultAsBusinessRule is not null)
                            {
                                validationContext.Remove(validationResultAsBusinessRule);
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(change.ChangeType));
                }
            }

            return changes;
        }
    }
}
