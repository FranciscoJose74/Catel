﻿namespace Catel.Services
{
    using System;
    using System.IO;

    /// <summary>
    /// <see cref="EventArgs"/> implementation for camera content ready operations.
    /// </summary>
    public class ContentReadyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentReadyEventArgs"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="imageStream"/> is <c>null</c>.</exception>
        public ContentReadyEventArgs(Stream imageStream)
        {
            ArgumentNullException.ThrowIfNull(imageStream);

            ImageStream = imageStream;
        }

        /// <summary>
        /// Gets the image stream of the image.
        /// </summary>
        /// <value>The image stream.</value>
        public Stream ImageStream { get; private set; }
    }
}
