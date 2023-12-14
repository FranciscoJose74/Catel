﻿namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using Catel.Data;

    public abstract class AbstractBase : ModelBase
    {
    }

    public class Derived1 : AbstractBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", "Test name");

    }

    public class Derived2 : AbstractBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", "Test name");

    }
}
