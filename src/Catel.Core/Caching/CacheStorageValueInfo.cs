﻿namespace Catel.Caching
{
    using System;
    using Policies;

    /// <summary>
    /// Value info for the cache storage.
    /// </summary>
    /// <typeparam name="TValue">
    /// The value type.
    /// </typeparam>
    internal class CacheStorageValueInfo<TValue>
    {
        private readonly ExpirationPolicy? _expirationPolicy;
        private readonly TValue _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStorageValueInfo{TValue}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        public CacheStorageValueInfo(TValue value, TimeSpan expiration)
            : this(value, ExpirationPolicy.Duration(expiration))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStorageValueInfo{TValue}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        public CacheStorageValueInfo(TValue value, ExpirationPolicy? expirationPolicy = null)
        {
            _value = value;
            _expirationPolicy = expirationPolicy;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value
        {
            get
            {
                if (_expirationPolicy is not null)
                {
                    if (CanExpire && _expirationPolicy.CanReset)
                    {
                        _expirationPolicy.Reset();
                    }
                }

                return _value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this value can expire.
        /// </summary>
        /// <value><c>true</c> if this value can expire; otherwise, <c>false</c>.</value>
        public bool CanExpire
        {
            get
            {
                return _expirationPolicy is not null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this value is expired.
        /// </summary>
        /// <value><c>true</c> if this value is expired; otherwise, <c>false</c>.</value>
        public bool IsExpired
        {
            get
            {
                if (_expirationPolicy is not null)
                {
                    return CanExpire && _expirationPolicy.IsExpired;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the expiration policy.
        /// </summary>
        internal ExpirationPolicy? ExpirationPolicy
        {
            get
            {
                return _expirationPolicy;
            }
        }

        /// <summary>
        /// Dispose value.
        /// </summary>
        public void DisposeValue()
        {
            var disposable = _value as IDisposable;
            if (disposable is not null)
            {
                disposable.Dispose();
            }
        }
    }
}
