﻿namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    [Serializable]
    public class ObjectWithCustomType : ComparableModelBase
    {
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithCustomType()
        {
        }

        /// <summary>
        ///   Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        ///   Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);

        /// <summary>
        ///   Gets or sets the gender.
        /// </summary>
        public Gender Gender
        {
            get { return GetValue<Gender>(GenderProperty); }
            set { SetValue(GenderProperty, value); }
        }

        /// <summary>
        ///   Register the Gender property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData GenderProperty = RegisterProperty("Gender", Gender.Male);
    }
}
