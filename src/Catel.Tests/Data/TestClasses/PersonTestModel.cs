﻿namespace Catel.Tests.Data
{
    using Catel.Data;

    public class PersonTestModel : ModelBase
    {
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);


        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", string.Empty);


        public bool IsEnabled
        {
            get { return GetValue<bool>(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly IPropertyData IsEnabledProperty = RegisterProperty("IsEnabled", false);
    }
}
