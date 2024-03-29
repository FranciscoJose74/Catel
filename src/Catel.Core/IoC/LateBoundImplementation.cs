﻿namespace Catel.IoC
{
    /// <summary>
    /// Class representing a late-bound implementation. This means that a type registered in the 
    /// <see cref="ServiceLocator"/> is registered with an unknown callback and the implementation
    /// type could not be determined at registration time.
    /// </summary>
    public sealed class LateBoundImplementation
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="LateBoundImplementation"/> class from being created.
        /// </summary>
        private LateBoundImplementation()
        {
            
        }
    }
}
