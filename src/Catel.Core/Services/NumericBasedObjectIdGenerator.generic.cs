﻿namespace Catel.Services
{
    using Catel.Reflection;

    /// <summary>
    /// The numeric based object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">The object type.</typeparam>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type.</typeparam>
    public abstract class NumericBasedObjectIdGenerator<TObjectType, TUniqueIdentifier> : ObjectIdGenerator<TObjectType, TUniqueIdentifier>
        where TObjectType : class
        where TUniqueIdentifier : notnull
    {
        static NumericBasedObjectIdGenerator()
        {
            Argument.IsValid("TUniqueIdentifier", typeof(TUniqueIdentifier), type => typeof(int).IsAssignableFromEx(type) || typeof(long).IsAssignableFromEx(type) || typeof(ulong).IsAssignableFromEx(type));
        }

        /// <summary>
        /// Gets and sets the value.
        /// </summary>
        protected static TUniqueIdentifier Value { get; set; } = default!;
    }
}
