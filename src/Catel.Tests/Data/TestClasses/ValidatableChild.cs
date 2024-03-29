﻿namespace Catel.Tests.Data.TestClasses
{
    using System.Collections.Generic;
    using Catel.Data;

    public class ValidatableChild : ValidatableModelBase
    {
        public ValidatableChild()
        {
        }

        public string Name
        {
            get => GetValue<string>(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly IPropertyData NameProperty = RegisterProperty<ValidatableChild, string>(model => model.Name, "Geert");


        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("B: There is no name"));
            }
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "F: There is no name"));
            }
        }
    }
}
