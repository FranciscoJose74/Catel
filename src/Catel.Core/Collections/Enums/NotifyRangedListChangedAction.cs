﻿namespace Catel.Collections
{
    /// <summary>
    /// Describes the real action performed on the <see cref="FastBindingList{T}"/>. 
    /// </summary>
    public enum NotifyRangedListChangedAction
    {
        /// <summary>
        /// Items was added to the <see cref="FastBindingList{T}"/>.
        /// </summary>
        Add,

        /// <summary>
        /// Items was removed from the <see cref="FastBindingList{T}"/>.
        /// </summary>
        Remove,

        /// <summary>
        /// The <see cref="FastBindingList{T}"/> has been reset.
        /// </summary>
        Reset
    }
}
