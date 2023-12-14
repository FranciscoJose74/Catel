﻿namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class SerializationObjectFacts
    {
        [TestFixture]
        public class TheFailedToDeserializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                Assert.Throws<ArgumentNullException>(() => SerializationObject.FailedToDeserialize(null, SerializationMemberGroup.CatelProperty, "property"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                Assert.Throws<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null));
                Assert.Throws<ArgumentException>(() => SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty));
            }
        }

        [TestFixture]
        public class TheSucceededToDeserializeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                Assert.Throws<ArgumentNullException>(() => SerializationObject.SucceededToDeserialize(null, SerializationMemberGroup.CatelProperty, "property", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                Assert.Throws<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, null, null));
                Assert.Throws<ArgumentException>(() => SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, string.Empty, null));
            }
        }

        [TestFixture]
        public class ThePropertyValueProperty
        {
            [TestCase]
            public void ThrowsInvalidOperationExceptionForFailedDeserialization()
            {
                var serializationObject = SerializationObject.FailedToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property");
                object propertyValue = null;

                Assert.Throws<InvalidOperationException>(() => propertyValue = serializationObject.MemberValue);

                Assert.That(propertyValue, Is.Null);
            }

            [TestCase]
            public void ReturnsPropertyValueForSucceededDeserialization()
            {
                var serializationObject = SerializationObject.SucceededToDeserialize(typeof(SerializationObject), SerializationMemberGroup.CatelProperty, "property", 42);
                object propertyValue = serializationObject.MemberValue;

                Assert.That(propertyValue, Is.EqualTo(42));
            }
        }
    }
}
