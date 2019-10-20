﻿namespace Catel.Tests.Data.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Data;
    using NUnit.Framework;

    public class ReflectionObjectAdapterFacts
    {
        public class TestClassWithRegularMembers
        {
            public string StringField;

            public string StringProperty { get; set; }
        }

        [TestFixture]
        public class TheSetValueMethod
        {
            [TestCase]
            public void SetsCatelModelPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new PersonTestModel();

                Assert.IsTrue(adapter.SetMemberValue(model, nameof(PersonTestModel.FirstName), "John"));
                Assert.AreEqual("John", model.FirstName);
            }

            [TestCase]
            public void SetsRegularPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsTrue(adapter.SetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), "John"));
                Assert.AreEqual("John", model.StringProperty);
            }

            [TestCase]
            public void SetsFieldValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsTrue(adapter.SetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), "John"));
                Assert.AreEqual("John", model.StringField);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsFalse(adapter.SetMemberValue(model, "NotExistingMember", "John"));
            }
        }

        [TestFixture]
        public class TheGetValueMethod
        {
            [TestCase]
            public void GetsCatelModelPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new PersonTestModel
                {
                    FirstName = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.GetMemberValue(model, nameof(PersonTestModel.FirstName), ref value));
                Assert.AreEqual("John", model.FirstName);
            }

            [TestCase]
            public void GetsRegularPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.GetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), ref value));
                Assert.AreEqual("John", value);
            }

            [TestCase]
            public void GetsFieldValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.GetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), ref value));
                Assert.AreEqual("John", value);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();
                string value = string.Empty;

                Assert.IsFalse(adapter.GetMemberValue(model, "NotExistingMember", ref value));
            }
        }
    }
}
