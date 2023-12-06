﻿namespace Catel.Tests.Reflection
{
    using System;
    using Catel.Reflection;
    using NUnit.Framework;

    public partial class PropertyHelperFacts
    {
        #region Test classes
        public class MyPropertyHelperClass
        {
            #region Fields
            private readonly int _privateWriteField;
            private int _privateReadField;
            #endregion

            #region Constructors
            public MyPropertyHelperClass()
            {
                PublicProperty = 1;
                _privateReadField = 2;
                _privateWriteField = 4;
            }
            #endregion

            #region Properties
            public int PublicProperty { get; set; }

            public int PrivateReadProperty
            {
                set { _privateReadField = value; }
            }

            public int PrivateWriteProperty
            {
                get { return _privateWriteField; }
            }

            public string StringValue { get; set; }
            #endregion

            #region Methods
            public int GetPrivateReadPropertyValue()
            {
                return _privateReadField;
            }
            #endregion
        }
        #endregion

        [TestFixture]
        public class TheIsPropertyAvailableMethod
        {
            [TestCase]
            public void IsPropertyAvailable_NullInput()
            {
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.IsPropertyAvailable(null, "PublicProperty"));
            }

            [TestCase]
            public void IsPropertyAvailable_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.That(PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "NotExistingProperty"), Is.EqualTo(false));
            }

            [TestCase]
            public void IsPropertyAvailable_ExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.That(PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "PublicProperty"), Is.EqualTo(true));
            }

            [TestCase("publicProperty", false, false)]
            [TestCase("publicProperty", true, true)]
            public void IsPropertyAvailable_ExistingProperty_IgnoreCase(string property, bool ignoreCase, bool expectedResult)
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();

                Assert.That(PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, property, ignoreCase), Is.EqualTo(expectedResult));
            }
        }

        [TestFixture]
        public class TheTryGetPropertyValueMethod
        {
            [TestCase]
            public void TryGetPropertyValue_ObjectNull()
            {
                object value;
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.TryGetPropertyValue(null, "property", out value));
            }

            [TestCase]
            public void TryGetPropertyValue_PropertyNameNull()
            {
                object value;
                var obj = new MyPropertyHelperClass();
                Assert.Throws<ArgumentException>(() => PropertyHelper.TryGetPropertyValue(obj, null, out value));
            }

            [TestCase]
            public void TryGetPropertyValue_NonExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "Non-existing property", out value);

                Assert.That(result, Is.False);
                Assert.That(value, Is.EqualTo(null));
            }

            [TestCase]
            public void TryGetPropertyValue_ExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "PublicProperty", out value);

                Assert.That(result, Is.True);
                Assert.That(value, Is.EqualTo(1));
            }

            [TestCase("publicProperty", false, false)]
            [TestCase("publicProperty", true, true)]
            public void TryGetPropertyValue_ExistingProperty_IgnoreCase(string property, bool ignoreCase, bool expectedResult)
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, property, ignoreCase, out value);

                Assert.That(result, Is.EqualTo(expectedResult));

                if (expectedResult)
                {
                    Assert.That(value, Is.EqualTo(1));
                }
            }
        }

        [TestFixture]
        public class TheGetPropertyValueMethod
        {
            [TestCase]
            public void GetPropertyValue_NullInput()
            {
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.GetPropertyValue(null, "PublicProperty"));
            }

            [TestCase]
            public void GetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.Throws<PropertyNotFoundException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "NotExistingProperty"));
            }

            [TestCase]
            public void GetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.Throws<CannotGetPropertyValueException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "PrivateReadProperty"));
            }

            [TestCase]
            public void GetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.That(PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PrivateWriteProperty"), Is.EqualTo(4));
            }

            [TestCase]
            public void GetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.PublicProperty = 42;
                Assert.That(PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PublicProperty"), Is.EqualTo(42));
            }

            [TestCase]
            public void GetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.StringValue = "FourtyTwo";
                Assert.That(PropertyHelper.GetPropertyValue<string>(myPropertyHelperClass, "StringValue"), Is.EqualTo("FourtyTwo"));
            }

            [TestCase("stringValue", false, "exception")]
            [TestCase("stringValue", true, "FourtyTwo")]
            public void GetPropertyValue_StringValue_IgnoreCase(string property, bool ignoreCase, string expectedResult)
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.StringValue = expectedResult;

                if (expectedResult == "exception")
                {
                    Assert.Throws<PropertyNotFoundException>(() => PropertyHelper.GetPropertyValue<string>(myPropertyHelperClass, property, ignoreCase));
                }
                else
                {
                    var result = PropertyHelper.GetPropertyValue<string>(myPropertyHelperClass, property, ignoreCase);
                    Assert.That(result, Is.EqualTo(expectedResult));
                }
            }
        }

        [TestFixture]
        public class TrySetPropertyValueMethod
        {
            [TestCase]
            public void TrySetPropertyValue_ObjectNull()
            {
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.TrySetPropertyValue(null, "property", null));
            }

            [TestCase]
            public void TrySetPropertyValue_PropertyNameNull()
            {
                var obj = new MyPropertyHelperClass();
                Assert.Throws<ArgumentException>(() => PropertyHelper.TrySetPropertyValue(obj, null, null));
            }

            [TestCase]
            public void TrySetPropertyValue_NonExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "Non-existing property", null);

                Assert.That(result, Is.False);
            }

            [TestCase]
            public void TrySetPropertyValue_ExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "PublicProperty", 5);

                Assert.That(result, Is.True);
                Assert.That(obj.PublicProperty, Is.EqualTo(5));
            }

            [TestCase("publicProperty", false, false)]
            [TestCase("publicProperty", true, true)]
            public void TrySetPropertyValue_ExistingProperty_IgnoreCase(string property, bool ignoreCase, bool expectedResult)
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, property, 5, ignoreCase);

                Assert.That(expectedResult, Is.EqualTo(result));
                if (expectedResult)
                {
                    Assert.That(obj.PublicProperty, Is.EqualTo(5));
                }
            }
        }

        [TestFixture]
        public class TheSetPropertyValueMethod
        {
            [TestCase]
            public void SetPropertyValue_NullInput()
            {
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.SetPropertyValue(null, "PublicProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.Throws<PropertyNotFoundException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "NotExistingProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateReadProperty", 42);
                Assert.That(myPropertyHelperClass.GetPrivateReadPropertyValue(), Is.EqualTo(42));
            }

            [TestCase]
            public void SetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.Throws<CannotSetPropertyValueException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateWriteProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PublicProperty", 42);
                Assert.That(myPropertyHelperClass.PublicProperty, Is.EqualTo(42));
            }

            [TestCase]
            public void SetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "StringValue", "FourtyTwo");
                Assert.That(myPropertyHelperClass.StringValue, Is.EqualTo("FourtyTwo"));
            }

            [TestCase("stringValue", false, false)]
            [TestCase("stringValue", true, true)]
            public void SetPropertyValue_StringValue_IgnoreCase(string property, bool ignoreCase, bool expectedResult)
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();

                if (!expectedResult)
                {
                    Assert.Throws<PropertyNotFoundException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, property, "FourtyTwo", ignoreCase));
                }
                else
                {
                    PropertyHelper.SetPropertyValue(myPropertyHelperClass, property, "FourtyTwo", ignoreCase);
                    Assert.That(myPropertyHelperClass.StringValue, Is.EqualTo("FourtyTwo"));
                }
            }
        }
    }
}