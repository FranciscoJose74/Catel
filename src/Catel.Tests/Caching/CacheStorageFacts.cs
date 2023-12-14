﻿namespace Catel.Tests.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;

    using NUnit.Framework;

    public class CacheStorageFacts
    {
        [TestFixture]
        public class TheThreadSafeFunctionality
        {
            private static readonly ILog Log = LogManager.GetCurrentClassLogger();
            private readonly List<Guid> _randomGuids = new List<Guid>();

            public TheThreadSafeFunctionality()
            {
                for (var i = 0; i < 10; i++)
                {
                    _randomGuids.Add(Guid.NewGuid());
                }
            }

            [TestCase]
            public void RunMultipleThreadsUsingGetFromCacheOrFetch()
            {
                RunMultipleThreadsWithRandomAccessCalls((cache, key) =>
                {
                    return cache.GetFromCacheOrFetch(key, () =>
                    {
                        var threadId = ThreadHelper.GetCurrentThreadId();
                        //Log.Info("Key '{0}' is now controlled by thread '{1}'", key, threadId);
                        return threadId;
                    });
                });
            }

            [TestCase]
            public void RunMultipleThreadsUsingAddAndGet()
            {
                RunMultipleThreadsWithRandomAccessCalls((cache, key) =>
                {
                    var threadId = ThreadHelper.GetCurrentThreadId();

                    cache.Add(key, threadId);

                    //Log.Info("Key '{0}' is now controlled by thread '{1}'", key, threadId);

                    return cache.Get(key);
                });
            }

            private void RunMultipleThreadsWithRandomAccessCalls(Func<ICacheStorage<Guid, int>, Guid, int> retrievalFunc)
            {
                var cacheStorage = new CacheStorage<Guid, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(250)));

                var threads = new List<Thread>();
                for (var i = 0; i < 50; i++)
                {
                    var thread = new Thread(() =>
                    {
                        var random = new Random();

                        for (var j = 0; j < 1000; j++)
                        {
                            var randomGuid = _randomGuids[random.Next(0, 9)];

                            retrievalFunc(cacheStorage, randomGuid);

                            ThreadHelper.Sleep(10);
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                while (true)
                {
                    var anyThreadAlive = false;

                    foreach (var thread in threads)
                    {
                        if (thread.IsAlive)
                        {
                            anyThreadAlive = true;
                            break;
                        }
                    }

                    if (!anyThreadAlive)
                    {
                        break;
                    }

                    ThreadHelper.Sleep(500);
                }
            }
        }

        [TestFixture]
        public class TheIndexerProperty
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var value = cache[null];
                    Assert.That(value, Is.Null);
                });
            }

            [TestCase]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, object>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache["2"], Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheGetMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var value = cache.Get(null);
                    Assert.That(value, Is.Null);
                });
            }

            [TestCase]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, object>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Get("2"), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheContainsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() => cache.Contains(null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Contains("3"), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Contains("2"), Is.True);
            }
        }

        [TestFixture]
        public class TheGetFromCacheOrFetchMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.GetFromCacheOrFetch(null, () => 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullFunction()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.GetFromCacheOrFetch("1", null));
            }

            [TestCase]
            public void AddsItemToCacheAndReturnsIt()
            {
                var cache = new CacheStorage<string, int>();

                var value = cache.GetFromCacheOrFetch("1", () => 1);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(1));
                Assert.That(value, Is.EqualTo(1));
            }

            [TestCase]
            public void ReturnsCachedItem()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(1));
                Assert.That(value, Is.EqualTo(1));
            }

            [TestCase]
            public void AddsItemToCacheWithOverrideAndReturnsIt()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2, true);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(2));
                Assert.That(value, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.Add(null, 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValueIfNotAllowNullValues()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() => cache.Add(null, null));
            }

            [TestCase]
            public void AddsNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.That(cache["1"], Is.EqualTo(1));
            }

            [TestCase]
            public void AddsNonExistingValueForTrueOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, true);

                Assert.That(cache["1"], Is.EqualTo(2));
            }

            [TestCase]
            public void DoesNotAddExistingValueForFalseOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, false);

                Assert.That(cache["1"], Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.Remove(null));
            }

            [TestCase]
            public void RemovesExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.That(cache.Contains("1"), Is.True);

                cache.Remove("1");

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void RemovesNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Remove("1");
            }

            [TestCase]
            public void DoesNotRaiseExpiringEventOnItemRemoval()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expiring += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }

            [TestCase]
            public void DoesNotRaiseExpiredEventOnItemRemoval()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expired += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheClearMethod
        {
            [TestCase]
            public void DoesNotRaiseExpiringEventOnClearStorage()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expiring += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }

            [TestCase]
            public void DoesNotRaiseExpiredEventOnClearStorage()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expired += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheAutoExpireFunctionality
        {
            [TestCase]
            public void IsAutomaticallyEnabledWhenStartedDisabledButAddingItemWithCustomExpirationPolicy()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void AutomaticallyRemovesExpiredItems()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void AutomaticallyRemovesExpiredItemsOfACacheStorageWithDefaultExpirationPolicyInitializationCode()
            {
                var cache = new CacheStorage<string, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(250)));
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1);

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void AddsAndExpiresSeveralItems()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                for (int i = 0; i < 5; i++)
                {
                    ThreadHelper.Sleep(1000);

                    int innerI = i;
                    var value = cache.GetFromCacheOrFetch("key", () => innerI, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(value, Is.EqualTo(i));
                }
            }

            [TestCase]
            public void RaisesExpiringEventWithCorrectEventArgsWhenItemExpires()
            {
                var key = "1";
                var expirationPolicy = new SlidingExpirationPolicy(TimeSpan.FromMilliseconds(250));
                var value = 1;
                var evKey = (string)null;
                var evExpirationPolicy = default(ExpirationPolicy);
                var evValue = 0;

                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expiring += (sender, e) =>
                {
                    evKey = e.Key;
                    evExpirationPolicy = e.ExpirationPolicy;
                    evValue = e.Value;
                };

                cache.Add(key, value, expirationPolicy);

                ThreadHelper.Sleep(750);

                Assert.That(evKey, Is.EqualTo(key));
                Assert.That(evExpirationPolicy, Is.EqualTo(expirationPolicy));
                Assert.That(evValue, Is.EqualTo(value));
            }

            [TestCase]
            public void ItemStaysInCacheWhenExpiringEventIsCanceled()
            {
                var key = "1";
                var value = 1;

                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expiring += (sender, e) =>
                {
                    e.Cancel = true;
                };

                cache.Add(key, value, expiration: new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains(key), Is.True);
            }

            [TestCase]
            public void RaisesExpiredEventWithCorrectEventArgsWhenItemExpires()
            {
                var dispose = true;
                var key = "1";
                var value = 1;
                var evDispose = false;
                var evKey = (string)null;
                var evValue = 0;

                var cache = new CacheStorage<string, int>();
                cache.DisposeValuesOnRemoval = dispose;
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expired += (sender, e) =>
                {
                    evDispose = e.Dispose;
                    evKey = e.Key;
                    evValue = e.Value;
                };

                cache.Add(key, value, expiration: new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(750);

                Assert.That(evDispose, Is.EqualTo(dispose));
                Assert.That(evKey, Is.EqualTo(key));
                Assert.That(evValue, Is.EqualTo(value));
            }
        }

        [TestFixture]
        public class TheDisposeItemsOnRemovalFunctionality
        {
            private sealed class CustomDisposable : IDisposable
            {
                public CustomDisposable()
                {
                    IsDiposed = false;
                }

                public bool IsDiposed { get; private set; }

                public void Dispose()
                {
                    IsDiposed = true;
                }
            }

            [TestCase]
            public void DisposesExpiredItemsWhenDisposingEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDiposed, Is.True);
                }
            }

            [TestCase]
            public void DisposesItemOnRemoveWhenDisposingEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    cache.Remove("disposable");

                    Assert.That(disposable.IsDiposed, Is.True);
                }
            }

            [TestCase]
            public void DisposesItemsOnClearWhenDisposingEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    cache.Clear();

                    Assert.That(disposable.IsDiposed, Is.True);
                }
            }

            [TestCase]
            public void DoesNotDisposeExpiredItemWhenDisposingEnabledButCanceledByEventArgs()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                    cache.Expired += (sender, e) =>
                    {
                        e.Dispose = false;
                    };

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDiposed, Is.False);
                }
            }

            [TestCase]
            public void DoesNotDisposeExpiredItemWhenDisposingNotEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDiposed, Is.False);
                }
            }

            [TestCase]
            public void DoesNotDisposeItemOnRemoveWhenDisposingNotEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    cache.Remove("disposable");

                    Assert.That(disposable.IsDiposed, Is.False);
                }
            }

            [TestCase]
            public void DoesNotDisposeItemsOnClearWhenDisposingNotEnabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);

                    cache.Clear();

                    Assert.That(disposable.IsDiposed, Is.False);
                }
            }

            [TestCase]
            public void DisposesExpiredItemWhenDisposingNotEnabledButForcedByEventArgs()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                    cache.Expired += (sender, e) =>
                    {
                        e.Dispose = true;
                    };

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDiposed, Is.True);
                }
            }
        }
    }
}
