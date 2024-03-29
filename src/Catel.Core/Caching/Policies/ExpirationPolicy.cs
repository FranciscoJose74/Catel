﻿namespace Catel.Caching.Policies
{
    using System;
    using Logging;

    /// <summary>
    /// The expiration policy.
    /// </summary>
    public abstract class ExpirationPolicy
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpirationPolicy" /> class.
        /// </summary>
        /// <param name="canReset">The can reset.</param>
        protected ExpirationPolicy(bool canReset = false)
        {
            CanReset = canReset;
        }

        /// <summary>
        /// Gets a value indicating whether the value with this policy attached is expired.
        /// </summary>
        public abstract bool IsExpired { get; }

        /// <summary>
        /// Gets a value indicating whether the value with this policy can be reset.
        /// </summary>
        public virtual bool CanReset { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is resting.
        /// </summary>
        protected bool IsResting { get; private set; }

        /// <summary>
        /// Creates a <see cref="AbsoluteExpirationPolicy" /> instance.
        /// </summary>
        /// <param name="absoluteExpirationDateTime">The absolute expiration <see cref="DateTime" />.</param>
        /// <returns>The <see cref="AbsoluteExpirationPolicy" /> or <c>null</c> if <paramref name="absoluteExpirationDateTime" /> is in the pass.</returns>
        /// <remarks>The cache item will expire on the absolute expiration date time.</remarks>
        public static ExpirationPolicy Absolute(DateTime absoluteExpirationDateTime)
        {
            return new AbsoluteExpirationPolicy(absoluteExpirationDateTime);
        }

        /// <summary>
        /// Creates a <see cref="DurationExpirationPolicy" /> instance.
        /// </summary>
        /// <param name="durationTimeSpan">The duration <see cref="TimeSpan" />.</param>
        /// <returns>The <see cref="DurationExpirationPolicy" /> or <c>null</c> if <paramref name="durationTimeSpan" /> is less than 0 ticks.</returns>
        /// <remarks>The cache item will expire using the duration to calculate the absolute expiration from now.</remarks>
        public static ExpirationPolicy Duration(TimeSpan durationTimeSpan)
        {
            return new DurationExpirationPolicy(durationTimeSpan);
        }

        /// <summary>
        /// Creates a <see cref="SlidingExpirationPolicy" /> instance.
        /// </summary>
        /// <param name="durationTimeSpan">The duration <see cref="TimeSpan" />.</param>
        /// <returns>The <see cref="SlidingExpirationPolicy" /> or <c>null</c> if <paramref name="durationTimeSpan" /> is less than 0 ticks.</returns>
        /// <remarks>The cache item will expire using the duration property as the sliding expiration.</remarks>
        public static ExpirationPolicy Sliding(TimeSpan durationTimeSpan)
        {
            return new SlidingExpirationPolicy(durationTimeSpan);
        }

        /// <summary>
        /// Creates a <see cref="CustomExpirationPolicy" /> instance.
        /// </summary>
        /// <param name="isExpiredFunc">The function to check if the policy is expired.</param>
        /// <param name="resetAction">The action that will be executed if the item is read before expiration.</param>
        /// <returns>The <see cref="CustomExpirationPolicy" />.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="isExpiredFunc" /> is <c>null</c>.</exception>
        public static ExpirationPolicy Custom(Func<bool> isExpiredFunc, Action? resetAction = null)
        {
            ArgumentNullException.ThrowIfNull(isExpiredFunc);

            return new CustomExpirationPolicy(isExpiredFunc, resetAction);
        }

        /// <summary>
        /// Resets the expiration policy.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the policy do not support this operation.</exception>
        public void Reset()
        {
            IsResting = true;

            try
            {
                if (!CanReset)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("This policy can't be reset");
                }

                OnReset();
            }
            finally
            {
                IsResting = false;
            }
        }

        /// <summary>
        /// Called when the policy is resetting.
        /// </summary>
        protected virtual void OnReset()
        {
        }
    }
}
