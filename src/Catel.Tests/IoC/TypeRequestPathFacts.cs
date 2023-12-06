﻿namespace Catel.Tests.IoC
{
    using System;
    using System.Linq;
    using Catel.IoC;
    using NUnit.Framework;

    public class TypeRequestPathFacts
    {
        private const string TestPathName = "TestPath";

        public class X
        {
            public X(Z z) { }
        }

        public class Y
        {
            public Y(X x) { }
        }

        public class Z
        {
            public Z(Y y) { }
        }

        private static TypeRequestInfo[] CreateArrayWithOnlyReferenceTypes()
        {
            return new TypeRequestInfo[]
            {
                new TypeRequestInfo(typeof(X)),
                new TypeRequestInfo(typeof(Y)),
                new TypeRequestInfo(typeof(Z))
            };
        }

        private static TypeRequestInfo[] CreateArrayWithOnlyValueTypes()
        {
            return new TypeRequestInfo[]
            {
                new TypeRequestInfo(typeof(int)),
                new TypeRequestInfo(typeof(double)),
                new TypeRequestInfo(typeof(DateTime))
            };
        }

        private static TypeRequestInfo[] CreateMixedArray()
        {
            return new TypeRequestInfo[]
            {
                new TypeRequestInfo(typeof(X)),
                new TypeRequestInfo(typeof(Y)),
                new TypeRequestInfo(typeof(Z))
            };
        }

        private static TypeRequestInfo[] CreateInvalidPath()
        {
            return new TypeRequestInfo[]
            {
                new TypeRequestInfo(typeof(X)),
                new TypeRequestInfo(typeof(Y)),
                new TypeRequestInfo(typeof(Z)),
                new TypeRequestInfo(typeof(X))
            };
        }

        private static TypeRequestPath MapRequestInfoArrayIntoPath(TypeRequestInfo[] typeRequestInfos)
        {
            var result = TypeRequestPath.Root(TestPathName);
            foreach (var typeRequestInfo in typeRequestInfos)
            {
                result = TypeRequestPath.Branch(result, typeRequestInfo);
            }
            return result;
        }

        [TestFixture]
        public class TheRoot
        {
            [TestCase]
            public void RootOfThePathCanBeCreatedWithoutTheName()
            {
                Assert.DoesNotThrow(() => TypeRequestPath.Root(string.Empty));
            }

            [TestCase]
            public void RootOfThePathCanBeCreatedWithName()
            {
                var path = TypeRequestPath.Root(TestPathName);
                Assert.That(path.Name, Is.EqualTo(TestPathName));
            }
        }

        [TestFixture]
        public class TheBranch
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParent()
            {
                var item = new TypeRequestInfo(typeof(X));
                Assert.Throws<ArgumentNullException>(() => TypeRequestPath.Branch(null, item));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullTypeRequestInfo()
            {
                var parent = TypeRequestPath.Root(string.Empty);
                Assert.Throws<ArgumentNullException>(() => TypeRequestPath.Branch(parent, null));
            }

            [TestCase]
            public void AddsTypeRequestInfosWithValueTypes()
            {
                var typeArray = CreateArrayWithOnlyValueTypes();

                var path = MapRequestInfoArrayIntoPath(typeArray);

                for (int i = 0; i < typeArray.Length; i++)
                {
                    Assert.That(path.AllTypes.ElementAt(i), Is.EqualTo(typeArray[i]));
                }

                Assert.That(path.FirstType, Is.EqualTo(typeArray[0]));
                Assert.That(path.LastType, Is.EqualTo(typeArray[typeArray.Length - 1]));
            }

            [TestCase]
            public void AddsTypeRequestInfosWithReferenceTypes()
            {
                var typeArray = CreateArrayWithOnlyReferenceTypes();

                var path = MapRequestInfoArrayIntoPath(typeArray);

                for (int i = 0; i < typeArray.Length; i++)
                {
                    Assert.That(path.AllTypes.ElementAt(i), Is.EqualTo(typeArray[i]));
                }

                Assert.That(path.FirstType, Is.EqualTo(typeArray[0]));
                Assert.That(path.LastType, Is.EqualTo(typeArray[typeArray.Length - 1]));
            }

            [TestCase]
            public void AddsTypeRequestInfosWithMixedTypes()
            {
                var typeArray = CreateMixedArray();

                var path = MapRequestInfoArrayIntoPath(typeArray);

                for (int i = 0; i < typeArray.Length; i++)
                {
                    Assert.That(path.AllTypes.ElementAt(i), Is.EqualTo(typeArray[i]));
                }

                Assert.That(path.FirstType, Is.EqualTo(typeArray[0]));
                Assert.That(path.LastType, Is.EqualTo(typeArray[typeArray.Length - 1]));
            }

            [TestCase]
            public void ThrowsCircularDependencyExceptionIfThereAreRepetitions()
            {
                var typeArray = CreateInvalidPath();
                Assert.Throws<CircularDependencyException>(() => MapRequestInfoArrayIntoPath(typeArray));
            }
        }

        [TestFixture]
        public class TheFirstTypeProperty
        {
            [TestCase]
            public void ReturnsNullForEmpty()
            {
                var path = TypeRequestPath.Root(string.Empty);
                Assert.That(path.FirstType, Is.Null);
            }

            [TestCase]
            public void ReturnsRightType()
            {
                var typeArray = CreateArrayWithOnlyValueTypes();
                var path = MapRequestInfoArrayIntoPath(typeArray);

                Assert.That(path.FirstType, Is.EqualTo(typeArray[0]));
            }
        }

        [TestFixture]
        public class TheLastTypeProperty
        {
            [TestCase]
            public void ReturnsNullForEmpty()
            {
                var path = TypeRequestPath.Root(string.Empty);
                Assert.That(path.LastType, Is.Null);
            }

            [TestCase]
            public void ReturnsRightType()
            {
                var typeArray = CreateArrayWithOnlyValueTypes();
                var path = MapRequestInfoArrayIntoPath(typeArray);

                Assert.That(path.LastType, Is.EqualTo(typeArray[typeArray.Length - 1]));
            }
        }

        [TestFixture]
        public class ToStringMethod
        {
            [TestCase]
            public void PathOfThreeTypesReturnExpectedString()
            {
                var typeArray = CreateArrayWithOnlyReferenceTypes();
                var path = MapRequestInfoArrayIntoPath(typeArray);

                const string expected = "X => Y => Z";
                Assert.That(path.ToString(), Is.EqualTo(expected));
            }
        }
    }
}
