﻿namespace Catel.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Catel.Collections;
    using Catel.Reflection;
    using NUnit.Framework;

    public class FastBindingListFacts
    {
        [TestFixture]
        public class TheIsDirtyProperty
        {
            [TestCase]
            public void ReturnsFalseWhenChangesAreNotSuspended()
            {
                var fastCollection = new FastBindingList<int>();

                Assert.That(fastCollection.IsDirty, Is.False);
            }

            [TestCase]
            public void ReturnsTrueWhenChangesAreSuspended()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications())
                {
                    Assert.That(fastCollection.IsDirty, Is.True);
                }

                Assert.That(fastCollection.IsDirty, Is.False);
            }
        }

        [TestFixture]
        public class TheNotificationsSuspendedProperty
        {
            [TestCase]
            public void ReturnsFalseAfterDisposing()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);

                    Assert.That(fastCollection.NotificationsSuspended, Is.True);
                }

                Assert.That(fastCollection.NotificationsSuspended, Is.False);
            }

            [TestCase]
            public void ReturnsFalseAfterChangedDisposing()
            {
                var fastCollection = new FastBindingList<int>();

                var firstToken = fastCollection.SuspendChangeNotifications();
                var secondToken = fastCollection.SuspendChangeNotifications();

#pragma warning disable IDISP017 // Prefer using.
                firstToken.Dispose();
                secondToken.Dispose();
#pragma warning restore IDISP017 // Prefer using.

                Assert.That(fastCollection.NotificationsSuspended, Is.False);
            }
        }

        [TestFixture]
        public class TheAddRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                Assert.Throws<ArgumentNullException>(() => fastCollection.AddItems(null));
            }

            [TestCase]
            public void RaisesSingleEventWhileAddingRange()
            {
                int counter = 0;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) => counter++;

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.That(counter, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheInsertRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                Assert.Throws<ArgumentNullException>(() => fastCollection.InsertItems(null, 0));
            }

            [TestCase]
            public void RaisesSingleEventWhileAddingRange()
            {
                int counter = 0;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) => counter++;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0);

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.InsertItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), 0);

                Assert.That(counter, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheRemoveRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                Assert.Throws<ArgumentNullException>(() => fastCollection.RemoveItems(null));
            }

            [TestCase]
            public void RaisesSingleEventWhileRemovingRange()
            {
                int counter = 0;

                var fastCollection = new FastBindingList<int>(new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 })
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) => counter++;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.RemoveItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.That(counter, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheSuspendNotificationsMethod
        {
            [TestCase]
            public void SuspendsValidationWhileAddingAndRemovingItems()
            {
                int counter = 0;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);
                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);

                    fastCollection.Remove(5);
                    fastCollection.Remove(4);
                    fastCollection.Remove(3);
                    fastCollection.Remove(2);
                    fastCollection.Remove(1);
                }

                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class SupportsLinq
        {
            [TestCase]
            public void ReturnsSingleElementUsingLinq()
            {
                var fastCollection = new FastBindingList<int>();

                for (int i = 0; i < 43; i++)
                {
                    fastCollection.Add(i);
                }

                var allInts = (from x in fastCollection
                               where x == 42
                               select x).FirstOrDefault();

                Assert.That(allInts, Is.EqualTo(42));
            }
        }

        [TestFixture]
        public class TheResetMethod
        {
            [TestCase]
            public void ResetWithoutSuspendChangeNotifications()
            {
                var collectionChanged = false;
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) =>
                {
                    collectionChanged = true;
                };

                fastCollection.Reset();

                Assert.That(collectionChanged, Is.EqualTo(true));
            }

            [TestCase]
            public void CallingResetWhileAddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyRangedListChangedEventArgs)null;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyRangedListChangedEventArgs;
                };

                using (var token = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);

                    fastCollection.Reset();
                    Assert.That(counter, Is.EqualTo(0));

                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
            }
        }

        [TestFixture]
        public class TheFindMethod
        {
            private class TestModel
            {
                public string TestProperty { get; set; }
            }

            [TestCase]
            public void ReturnsItemIndexWhenItemWasFound()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestProperty", false);

                var fastCollection = new FastBindingList<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.Add(new TestModel() { TestProperty = "Test1" });
                fastCollection.Add(new TestModel() { TestProperty = "Test2" });
                fastCollection.Add(new TestModel() { TestProperty = "Test3" });

                var idx0 = ((IBindingList)fastCollection).Find(desc, "Test1");
                var idx1 = ((IBindingList)fastCollection).Find(desc, "Test2");
                var idx2 = ((IBindingList)fastCollection).Find(desc, "Test3");

                Assert.That(idx0, Is.EqualTo(0));
                Assert.That(idx1, Is.EqualTo(1));
                Assert.That(idx2, Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsMinusOneWhenItemWasNotFound()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestProperty", false);

                var fastCollection = new FastBindingList<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.Add(new TestModel() { TestProperty = "Test1" });
                fastCollection.Add(new TestModel() { TestProperty = "Test2" });
                fastCollection.Add(new TestModel() { TestProperty = "Test3" });

                var idxnf = ((IBindingList)fastCollection).Find(desc, "Test4");

                Assert.That(idxnf, Is.EqualTo(-1));
            }
        }

        [TestFixture]
        public class TheSortMethod
        {
            private class TestType : IComparable
            {
                public int TestIntProperty { get; set; }

                public int CompareTo(object obj)
                {
                    var another = obj as TestType;
                    if (another is null)
                    {
                        return -1;
                    }

                    return Comparer<int>.Default.Compare(TestIntProperty, another.TestIntProperty);
                }
            }

            private class TestModel
            {
                public int TestIntProperty { get; set; }
                public string TestStringProperty { get; set; }
                public TestType TestTypeProperty { get; set; }
            }

            [TestCase]
            public void ListIsSortedAscendingAfterSortAscendingByIntField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestIntProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = null };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = null };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm2);
                fastCollection.Add(tm1);
                fastCollection.Add(tm0);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Ascending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tm0, tm1, tm2 }).AsCollection);
            }

            [TestCase]
            public void ListIsSortedAscendingAfterSortAscendingByStringField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestStringProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = null };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = null };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = null };
                var tmn = new TestModel() { TestIntProperty = 4, TestStringProperty = null, TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm2);
                fastCollection.Add(tm1);
                fastCollection.Add(tmn);
                fastCollection.Add(tm0);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Ascending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tmn, tm0, tm1, tm2 }).AsCollection);
            }

            [TestCase]
            public void ListIsSortedAscendingAfterSortAscendingByClassTypeField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestTypeProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = new TestType() { TestIntProperty = 1 } };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = new TestType() { TestIntProperty = 2 } };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = new TestType() { TestIntProperty = 3 } };
                var tmn = new TestModel() { TestIntProperty = 4, TestStringProperty = null, TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm2);
                fastCollection.Add(tm1);
                fastCollection.Add(tmn);
                fastCollection.Add(tm0);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Ascending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tmn, tm0, tm1, tm2 }).AsCollection);
            }

            [TestCase]
            public void ListIsSortedDescendingAfterSortDescendingByIntField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestIntProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = null };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = null };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm0);
                fastCollection.Add(tm1);
                fastCollection.Add(tm2);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Descending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tm2, tm1, tm0 }).AsCollection);
            }

            [TestCase]
            public void ListIsSortedDescendingAfterSortDescendingByStringField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestStringProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = null };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = null };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = null };
                var tmn = new TestModel() { TestIntProperty = 4, TestStringProperty = null, TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm0);
                fastCollection.Add(tm1);
                fastCollection.Add(tmn);
                fastCollection.Add(tm2);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Descending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tm2, tm1, tm0, tmn }).AsCollection);
            }

            [TestCase]
            public void ListIsSortedDescendingAfterSortDescendingByClassTypeField()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestTypeProperty", false);

                var fastCollection = new FastBindingList<TestModel>();
                var tm0 = new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1", TestTypeProperty = new TestType() { TestIntProperty = 1 } };
                var tm1 = new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2", TestTypeProperty = new TestType() { TestIntProperty = 2 } };
                var tm2 = new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3", TestTypeProperty = new TestType() { TestIntProperty = 3 } };
                var tmn = new TestModel() { TestIntProperty = 4, TestStringProperty = null, TestTypeProperty = null };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.Add(tm0);
                fastCollection.Add(tm1);
                fastCollection.Add(tmn);
                fastCollection.Add(tm2);

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Descending);

                Assert.That(fastCollection, Is.EqualTo(new List<TestModel> { tm2, tm1, tm0, tmn }).AsCollection);
            }

            [TestCase]
            public void RaisesResetEventWhileSorting()
            {
                var pdc = TypeDescriptor.GetProperties(typeof(TestModel));
                var desc = pdc.Find("TestStringProperty", false);

                var counter = 0;
                var eventArgs = (NotifyListChangedEventArgs)null;

                var fastCollection = new FastBindingList<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.Add(new TestModel() { TestIntProperty = 1, TestStringProperty = "Test1" });
                fastCollection.Add(new TestModel() { TestIntProperty = 2, TestStringProperty = "Test2" });
                fastCollection.Add(new TestModel() { TestIntProperty = 3, TestStringProperty = "Test3" });
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyListChangedEventArgs;
                };

                ((IBindingList)fastCollection).ApplySort(desc, ListSortDirection.Ascending);

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
            }
        }

        [TestFixture]
        public class TheSuspensionModeMethod
        {
            [TestCase]
            public void AddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyRangedListChangedEventArgs)null;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyRangedListChangedEventArgs;
                };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);
                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
            }

            [TestCase]
            public void CascadedAddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyRangedListChangedEventArgs)null;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyRangedListChangedEventArgs;
                };

                using (var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    using (var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                    {
                        fastCollection.Add(1);
                        fastCollection.Add(2);
                        fastCollection.Add(3);
                        fastCollection.Add(4);
                        fastCollection.Add(5);
                    }

                    Assert.That(counter, Is.EqualTo(0));
                    Assert.That(eventArgs, Is.Null);
                }

                Assert.That(counter, Is.EqualTo(1));
                // ReSharper disable PossibleNullReferenceException
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
                // ReSharper restore PossibleNullReferenceException
            }

            [TestCase]
            public void CascadedAddingItemsInAddingModeWithInterceptingDisposing()
            {
                var counter = 0;
                var eventArgs = (NotifyRangedListChangedEventArgs)null;

                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyRangedListChangedEventArgs;
                };

                using (var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    using (var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                    {
                        fastCollection.Add(1);
                        fastCollection.Add(2);
                    }

                    Assert.That(counter, Is.EqualTo(0));
                    Assert.That(eventArgs, Is.Null);

                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.That(counter, Is.EqualTo(1));
                // ReSharper disable PossibleNullReferenceException
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
                // ReSharper restore PossibleNullReferenceException
            }

            [TestCase]
            public void RemovingItemsInRemovingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyRangedListChangedEventArgs)null;

                var fastCollection = new FastBindingList<int> { 1, 2, 3, 4, 5 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.ListChanged += (sender, e) =>
                {
                    counter++;
                    eventArgs = e as NotifyRangedListChangedEventArgs;
                };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                {
                    fastCollection.Remove(1);
                    fastCollection.Remove(2);
                    fastCollection.Remove(3);
                    fastCollection.Remove(4);
                    fastCollection.Remove(5);
                }

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.ListChangedType, Is.EqualTo(ListChangedType.Reset));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Remove));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.OldItems).AsCollection);
            }

            private SuspensionContext<T> GetSuspensionContext<T>(FastBindingList<T> collection)
            {
                var t = typeof(FastBindingList<T>);
                var f = t.GetFieldEx("_suspensionContext", BindingFlags.Instance | BindingFlags.NonPublic);
                var v = f.GetValue(collection) as SuspensionContext<T>;

                return v;
            }

            [TestCase]
            public void CleanedUpSuspensionContextAfterAdding()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                }

                var context = GetSuspensionContext(fastCollection);
                Assert.That(context, Is.Null);
            }

            [TestCase]
            public void CleanedUpSuspensionContextAfterDoingNothing()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                }

                var context = GetSuspensionContext(fastCollection);
                Assert.That(context, Is.Null);
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForAddingInRemovingMode()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Add(0));
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForClearingInAddingMode()
            {
                var fastCollection = new FastBindingList<int>();

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Clear());
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForRemovingInAddingMode()
            {
                var fastCollection = new FastBindingList<int> { 0 };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Remove(0));
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForSettingInAddingMode()
            {
                var fastCollection = new FastBindingList<int> { 0 };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection[0] = 0);
                }
            }

            [TestCase]
            [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
            public void ThrowsInvalidOperationExceptionForChangingMode()
            {
                var fastCollection = new FastBindingList<int> { 0 };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => { using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing)) { } });
                }
            }
        }

        [TestFixture]
        public class TheMixedMode
        {
            [Test]
            public void RaisesSingleAddEventIfTheRemovedItemsAreASubSetOfTheAddedItems()
            {
                var count = 0;
                NotifyRangedListChangedEventArgs eventArgs = null;
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };

                fastCollection.ListChanged += (sender, args) =>
                {
                    count++;
                    eventArgs = args as NotifyRangedListChangedEventArgs;
                };

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.AddItems(new[] { 1, 2, 3, 4 });
                    fastCollection.RemoveItems(new[] { 2, 3 });
                }

                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Add));
                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.NewItems.OfType<int>().ToArray(), Is.EqualTo(new[] { 1, 4 }));
            }

            [Test]
            public void RaisesSingleRemoveEventIfTheAddedItemsAreASubSetOfTheRemovedItems()
            {
                var count = 0;
                NotifyRangedListChangedEventArgs eventArgs = null;
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.AddItems(new[] { 1, 2, 3, 4 });

                fastCollection.ListChanged += (sender, args) =>
                {
                    count++;
                    eventArgs = args as NotifyRangedListChangedEventArgs;
                };

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.RemoveItems(new[] { 4, 2, 3 });
                    fastCollection.AddItems(new[] { 2 });
                }

                Assert.That(eventArgs.Action, Is.EqualTo(NotifyRangedListChangedAction.Remove));
                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.OldItems.OfType<int>().ToArray(), Is.EqualTo(new[] { 4, 3 }));
            }

            [Test]
            public void RaisesTwoEvents()
            {
                var eventArgsList = new List<NotifyRangedListChangedEventArgs>();
                var fastCollection = new FastBindingList<int>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };
                fastCollection.AddItems(new[] { 1, 2, 3, 4 });

                fastCollection.ListChanged += (sender, args) =>
                {
                    eventArgsList.Add(args as NotifyRangedListChangedEventArgs);
                };

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.RemoveItems(new[] { 4 });
                    fastCollection.AddItems(new[] { 5 });
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));

                Assert.Contains(5, eventArgsList.First(args => args.Action == NotifyRangedListChangedAction.Add).NewItems);
                Assert.Contains(4, eventArgsList.First(args => args.Action == NotifyRangedListChangedAction.Remove).OldItems);
            }
        }
    }
}
