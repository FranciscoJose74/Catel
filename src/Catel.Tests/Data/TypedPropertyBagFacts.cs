﻿namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;
    using NUnit.Framework;

    [TestFixture]
    public class TypedPropertyBagFacts
    {
        //[TestCase]
        //public void CanSetCompatiblePropertyValues()
        //{
        //    var propertyBag = new TypedPropertyBag();

        //    propertyBag.SetValue("Int", 42);
        //    propertyBag.SetValue("Int", 42u);
        //}

        [TestCase]
        public void PreventsRegistrationWithNullValue()
        {
            var propertyBag = new TypedPropertyBag();

            propertyBag.SetValue("Int", 42);
            Assert.Throws<InvalidOperationException>(() => propertyBag.SetValue("Int", (object)null));
        }

        [TestCase]
        public void PreventsRegistrationWithDifferentTypes_1()
        {
            var propertyBag = new TypedPropertyBag();

            propertyBag.SetValue("Int", 42);
            Assert.Throws<InvalidCastException>(() => propertyBag.SetValue("Int", (object)true));
        }

        [TestCase]
        public void PreventsRegistrationWithDifferentTypes_2()
        {
            var propertyBag = new TypedPropertyBag();

            propertyBag.SetValue("Int", 42);
            Assert.Throws<InvalidCastException>(() => propertyBag.SetValue("Int", new object()));
        }

        [TestCase]
        public void AutomaticallyCastsObjectsToRightTypesIfPossible()
        {
            var propertyBag = new TypedPropertyBag();

            propertyBag.SetValue("Int", 42);
            propertyBag.SetValue("Int", (object)52);

            Assert.AreEqual(52, propertyBag.GetValue("Int", 0));
        }
    }
}
