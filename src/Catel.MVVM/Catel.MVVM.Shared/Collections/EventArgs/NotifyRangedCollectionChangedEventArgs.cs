// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedCollectionChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// The ranged notify collection changed event args.
    /// </summary>
    public class NotifyRangedCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        public NotifyRangedCollectionChangedEventArgs(SuspensionMode mode)
            : base(ModeToAction(mode))
        {
            SuspensionMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        public NotifyRangedCollectionChangedEventArgs(SuspensionMode mode, IList changedItems, IList<int> indices)
            : base(ModeToAction(mode), changedItems, (indices != null && indices.Count != 0) ? indices[0] : -1)
        {
            Argument.IsNotNull(nameof(indices), indices);
            // ReSharper disable once PossibleNullReferenceException
            Argument.IsNotOutOfRange(nameof(indices), indices.Count, changedItems.Count, changedItems.Count);

            SuspensionMode = mode;
            Indices = indices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <param name="mixedChangedItems">The mixed changed items.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="mixedActions">The mixed actions.</param>
        public NotifyRangedCollectionChangedEventArgs(SuspensionMode mode, IList mixedChangedItems, IList<int> indices, IList<NotifyCollectionChangedAction> mixedActions)
            : base(ModeToAction(mode))
        {
            Argument.IsNotNull(nameof(mixedChangedItems), mixedChangedItems);
            Argument.IsNotNull(nameof(indices), indices);
            Argument.IsNotNull(nameof(mixedActions), mixedActions);

            SuspensionMode = mode;
            MixedItems = mixedChangedItems;
            Indices = indices;
            MixedActions = mixedActions;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the indices.
        /// </summary>
        public IList<int> Indices { get; }

        /// <summary>
        /// Gets the mixed items.
        /// </summary>
        public IList MixedItems { get; }

        /// <summary>
        /// Gets the mixed actions.
        /// </summary>
        public IList<NotifyCollectionChangedAction> MixedActions { get; }

        /// <summary>
        /// Gets the suspension mode.
        /// </summary>
        public SuspensionMode SuspensionMode { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Transforms the <see cref="SuspensionMode"/> into its equivalent <see cref="NotifyCollectionChangedAction"/>.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <returns>The equivalent <see cref="NotifyCollectionChangedAction"/>.</returns>
        private static NotifyCollectionChangedAction ModeToAction(SuspensionMode mode)
        {
            switch (mode)
            {
                case SuspensionMode.None:
                case SuspensionMode.Mixed: return NotifyCollectionChangedAction.Reset;
                case SuspensionMode.Adding: return NotifyCollectionChangedAction.Add;
                case SuspensionMode.Removing: return NotifyCollectionChangedAction.Remove;
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        #endregion
    }
}