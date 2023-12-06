﻿namespace Catel.Tests.Caching.Policies
{
    using System;
    using System.Threading;

    using Catel.Caching.Policies;

    using NUnit.Framework;

    public class CompositeExpirationPolicyFacts
    {
        #region Nested type: TheCanResetProperty

        /// <summary>
        /// The can reset property.
        /// </summary>
        [TestFixture]
        public class TheCanResetProperty
        {
            #region Methods

            /// <summary>
            /// Returns true if any policy can be reset.
            /// </summary>
            [TestCase]
            public void ReturnsTrueIfAnyPolicyCanBeReset()
            {
                Assert.That(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true, () => ThreadHelper.Sleep(0))).Add(new CustomExpirationPolicy(() => true)).CanReset, Is.True);
            }

            /// <summary>
            /// Returns false if all policy can not be reset.
            /// </summary>
            [TestCase]
            public void ReturnsFalseIfAllPolicyCanNotBeReset()
            {
                Assert.That(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true)).CanReset, Is.False);
            }

            [TestCase]
            public void DoesNotCauseDeathLock()
            {
                CompositeExpirationPolicy policy = new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true));
                var events = new[] { new AutoResetEvent(false), new AutoResetEvent(false) };

                new Thread(() =>
                {
                    Assert.That(policy.CanReset, Is.False);
                    events[0].Set();
                }).Start();

                new Thread(() =>
                {
                    Assert.That(policy.CanReset, Is.False);
                    events[1].Set();
                }).Start();

                // How specify the apartment statte to with MSTest: 
                // WaitHandle.WaitAll(events,TimeSpan.FromSeconds(10))
                events[0].WaitOne(TimeSpan.FromSeconds(10));
                events[1].WaitOne(TimeSpan.FromSeconds(10));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsExpiredProperty

        /// <summary>
        /// The the is expired property.
        /// </summary>
        [TestFixture]
        public class TheIsExpiredProperty
        {
            #region Methods

            [TestCase]
            public void ReturnsTrueIfAnyPolicyExpires()
            {
                Assert.That(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => true)).IsExpired, Is.True);
            }


            [TestCase]
            public void ReturnsFalseIfAnyPolicyExpiresButWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.That(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => true)).IsExpired, Is.False);
            }


            [TestCase]
            public void ReturnsTrueIfAllPolicyExpiresButWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.That(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true)).IsExpired, Is.True);
            }

            [TestCase]
            public void ReturnsFalseIfAllPolicyNonExpiresWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.That(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => false)).IsExpired, Is.False);
            }

            [TestCase]
            public void ReturnsFalseIfAllPolicyNonExpires()
            {
                Assert.That(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => false)).IsExpired, Is.False);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheResetMethod

        /// <summary>
        /// The can reset property.
        /// </summary>
        [TestFixture]
        public class TheResetMethod
        {
            #region Methods


            /// <summary>
            /// Invokes the action.
            /// </summary>
            [TestCase]
            public void InvokesTheResetActions()
            {
                bool actionInvoked1 = false;
                bool actionInvoked2 = false;

                new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true, () => actionInvoked1 = true)).Add(new CustomExpirationPolicy(() => true, () => actionInvoked2 = true)).Reset();

                Assert.That(actionInvoked1, Is.True);
                Assert.That(actionInvoked2, Is.True);
            }

            [TestCase]
            public void DoesNotCauseDeathLockIfPolicyCanNotBeResetAndThrowsInvalidOperationException()
            {
                var events = new[] { new AutoResetEvent(false), new AutoResetEvent(false) };
                CompositeExpirationPolicy policy = new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true));

                new Thread(() =>
                    {
                        Assert.Throws<InvalidOperationException>(policy.Reset);
                        events[0].Set();
                    }).Start();

                new Thread(() =>
                    {
                        Assert.Throws<InvalidOperationException>(policy.Reset);
                        events[1].Set();
                    }).Start();

                // How specify the apartment statte to with MSTest: 
                // WaitHandle.WaitAll(events,TimeSpan.FromSeconds(10))
                events[0].WaitOne(TimeSpan.FromSeconds(10));
                events[1].WaitOne(TimeSpan.FromSeconds(10));
            }
            #endregion
        }
        #endregion
    }
}
