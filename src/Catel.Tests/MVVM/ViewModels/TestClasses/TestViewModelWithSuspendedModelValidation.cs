﻿namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithSuspendedModelValidation : ViewModelBase
    {
        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName");

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData MiddleNameProperty = RegisterProperty<string>("MiddleName");

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("LastName");

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData PersonProperty = RegisterProperty<IPerson>("Person");

        public TestViewModelWithSuspendedModelValidation(Person person)
        {
            ValidateModelsOnInitialization = true;
            Person = person;
            DeferValidationUntilFirstSaveCall = false;
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string FirstName
        {
            get => GetValue<string>(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string MiddleName
        {
            get => GetValue<string>(MiddleNameProperty);
            set => SetValue(MiddleNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string LastName
        {
            get => GetValue<string>(LastNameProperty);
            set => SetValue(LastNameProperty, value);
        }

        [Model(SupportValidation = false)]
        public IPerson Person
        {
            get => GetValue<IPerson>(PersonProperty);
            set => SetValue(PersonProperty, value);
        }
    }
}
