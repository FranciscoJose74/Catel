﻿namespace Catel.Tests.Data.TestClasses
{
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;

    public class SuspendableTestModel : ValidatableModelBase
    {
        [Required]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty, (sender, e) => ((SuspendableTestModel)sender).OnFirstNameChanged());

        public bool IsFirstNameCallbackInvoked { get; private set; }

        private void OnFirstNameChanged()
        {
            IsFirstNameCallbackInvoked = true;
        }

        [Required]
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", string.Empty, (sender, e) => ((SuspendableTestModel)sender).OnLastNameChanged());

        public bool IsLastNameCallbackInvoked { get; private set; }

        private void OnLastNameChanged()
        {
            IsLastNameCallbackInvoked = true;
        }
    }
}
