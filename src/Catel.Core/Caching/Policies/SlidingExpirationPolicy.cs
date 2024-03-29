﻿namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
    /// The cache item will expire using the duration property as the sliding expiration.
    /// </summary>
    public sealed class SlidingExpirationPolicy : DurationExpirationPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingExpirationPolicy"/> class.
        /// </summary>
        /// <param name="durationTimeSpan">
        /// The expiration.
        /// </param>
        internal SlidingExpirationPolicy(TimeSpan durationTimeSpan)
            : base(durationTimeSpan, true)
        {
        }

        /// <summary>
        /// The reset.
        /// </summary>
        protected override void OnReset()
        {
            AbsoluteExpirationDateTime = FastDateTime.Now.Add(DurationTimeSpan);
        }
    }
}
