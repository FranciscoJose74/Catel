﻿namespace Catel.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Catel.Collections;
    using NUnit.Framework;

    public class FastObservableDictionaryFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [Test]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                Assert.That(() => new FastObservableDictionary<object, object>(null, null), Throws.ArgumentNullException);
            }

            [Test]
            public void ReturnsDefaultComparer()
            {
                var defaultComparer = EqualityComparer<int>.Default;

                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                Assert.That(observableDictionary.Comparer, Is.EqualTo(defaultComparer));
            }

            [Test]
            public void ReturnsCustomComparer()
            {
                var customComparer = StringComparer.OrdinalIgnoreCase;

                var observableDictionary = new FastObservableDictionary<string, int>(customComparer)
                {
                    {
                        "1", 1
                    }
                };

                Assert.That(observableDictionary.Comparer, Is.EqualTo(customComparer));
            }
        }

        [TestFixture]
        public class TheAddMethod
        {
            [Test]
            public void ThrowsInvalidCastExceptionForAddObject()
            {
                var observableDictionary = new FastObservableDictionary<int, int>();

                Assert.That(() => observableDictionary.Add((object)"1", 1), Throws.TypeOf<InvalidCastException>());
            }

            [Test]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var observableDictionary = new FastObservableDictionary<object, int>();

                Assert.That(() => observableDictionary.Add(null, 1), Throws.ArgumentNullException);
            }

            [Test]
            public void AllowsNullValues()
            {
                var observableDictionary = new FastObservableDictionary<int, object>();

                observableDictionary.Add(1, null);

                Assert.That(observableDictionary[1], Is.Null);
            }

            [Test]
            public void RaisesEventWhileAddingKvp()
            {
                var counter = 0;
                var observableDictionary = new FastObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add(new KeyValuePair<int, int>(1, 1));
                Assert.That(counter, Is.EqualTo(1));

                observableDictionary.Add(new KeyValuePair<int, int>(2, 2));
                Assert.That(counter, Is.EqualTo(2));

                observableDictionary.Add(new KeyValuePair<int, int>(3, 3));
                Assert.That(counter, Is.EqualTo(3));

                observableDictionary.Add(new KeyValuePair<int, int>(4, 4));
                Assert.That(counter, Is.EqualTo(4));
            }

            [Test]
            public void RaisesEventWhileAddingObject()
            {
                var counter = 0;
                var observableDictionary = new FastObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add((object)1, (object)1);
                Assert.That(counter, Is.EqualTo(1));

                observableDictionary.Add((object)2, (object)2);
                Assert.That(counter, Is.EqualTo(2));

                observableDictionary.Add((object)3, (object)3);
                Assert.That(counter, Is.EqualTo(3));

                observableDictionary.Add((object)4, (object)4);
                Assert.That(counter, Is.EqualTo(4));
            }

            [Test]
            public void RaiseEventWhileAddingStronglyTyped()
            {
                var counter = 0;
                var observableDictionary = new FastObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add(1, 1);
                Assert.That(counter, Is.EqualTo(1));

                observableDictionary.Add(2, 2);
                Assert.That(counter, Is.EqualTo(2));

                observableDictionary.Add(3, 3);
                Assert.That(counter, Is.EqualTo(3));

                observableDictionary.Add(4, 4);
                Assert.That(counter, Is.EqualTo(4));
            }
        }

        [TestFixture]
        public class TheClearMethod
        {
            [Test]
            public void RaisesEventWhileClearing()
            {
                bool wasRaised = false;
                var observableDictionary = new FastObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => wasRaised = args.Action == NotifyCollectionChangedAction.Reset;

                observableDictionary.Clear();

                Assert.That(wasRaised && observableDictionary.Count == 0, Is.True);
            }
        }

        [TestFixture]
        public class TheContainsMethod
        {
            [Test]
            public void ContainsKvpSuccess()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains(new KeyValuePair<int, int>(1, 2));

                Assert.That(result, Is.True);
            }

            [Test]
            public void ContainsObjectTrue()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)1);

                Assert.That(result, Is.True);
            }

            [Test]
            public void ContainsObjectFalse()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)2);

                Assert.That(result, Is.False);
            }

            [Test]
            public void ContainsObjectInvalidTypeFalse()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)"1");

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class TheContainsKeyMethod
        {
            [Test]
            public void ContainsKeyTrue()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.ContainsKey(1);

                Assert.That(success, Is.True);
            }

            [Test]
            public void ContainsKeyFalse()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.ContainsKey(2);

                Assert.That(success, Is.False);
            }
        }

        [TestFixture]
        public class TheCopyToMethod
        {
            [Test]
            public void PopulatesArray()
            {
                Array arr = new KeyValuePair<int, int>[2];

                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    },
                    {
                        2, 2
                    }
                };

                observableDictionary.CopyTo(arr, 0);

                Assert.That(arr.Length, Is.EqualTo(2));
            }
            [Test]
            public void PopulatesKvpArray()
            {
                KeyValuePair<int, int>[] arr = new KeyValuePair<int, int>[2];

                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    },
                    {
                        2, 2
                    }
                };

                observableDictionary.CopyTo(arr, 0);

                Assert.That(arr.Length, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheIndexer
        {
            [Test]
            public void ThrowsInvalidCastExceptionForAddingObject()
            {
                var observableDictionary = new FastObservableDictionary<int, int>();

                Assert.That(() => observableDictionary[(object)"1"] = 1, Throws.TypeOf<InvalidCastException>());
            }

            [Test]
            public void RaisesEventWhileAddingObject()
            {
                var counter = 0;
                var observableDictionary = new FastObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary[(object)1] = 1;
                Assert.That(counter, Is.EqualTo(1));

                observableDictionary[(object)2] = 2;
                Assert.That(counter, Is.EqualTo(2));

                observableDictionary[(object)3] = 3;
                Assert.That(counter, Is.EqualTo(3));

                observableDictionary[(object)4] = 4;
                Assert.That(counter, Is.EqualTo(4));
            }

            [Test]
            public void RaisesEventWhileAddingStronglyTyped()
            {
                var counter = 0;
                var observableDictionary = new FastObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary[1] = 1;
                Assert.That(counter, Is.EqualTo(1));

                observableDictionary[2] = 2;
                Assert.That(counter, Is.EqualTo(2));

                observableDictionary[3] = 3;
                Assert.That(counter, Is.EqualTo(3));

                observableDictionary[4] = 4;
                Assert.That(counter, Is.EqualTo(4));
            }

            [Test]
            public void RaisesEventWhileUpdatingObject()
            {
                var isUpdated = false;
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => isUpdated = args.Action == NotifyCollectionChangedAction.Replace;

                observableDictionary[(object)1] = 3;

                Assert.That(isUpdated, Is.True);
            }

            [Test]
            public void RaisesEventWhileUpdatingStronglyTyped()
            {
                var isUpdated = false;
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => isUpdated = args.Action == NotifyCollectionChangedAction.Replace;

                observableDictionary[1] = 3;

                Assert.That(isUpdated, Is.True);
            }

            [Test]
            public void ReturnsNullForInvalidTypedObject()
            {
                var observableDictionary = new FastObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var result = observableDictionary[(object)"1"];

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [Test]
            public void RaiseEventWhileRemovingStronglyTyped()
            {
                var counter = 1;
                var observableDictionary = new FastObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter--;

                observableDictionary.Remove(1);
                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheTryGetValueMethod
        {
            [Test]
            public void ReturnsValueFromValidKey()
            {
                var observableDictionary = new FastObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.TryGetValue(1, out int value);

                Assert.That(success && value == 1, Is.True);
            }

            [Test]
            public void ReturnsDefaultFromInvalidKey()
            {
                var observableDictionary = new FastObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.TryGetValue(2, out int value);

                Assert.That(!success && value == 0, Is.True);
            }
        }
    }
}
