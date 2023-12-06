﻿namespace Catel.Tests.Services.Exceptions
{
    using Catel.Services;

    using NUnit.Framework;

    [TestFixture]
    public class WindowNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new WindowNotRegisteredException("windowName");
            }
            catch (WindowNotRegisteredException ex)
            {
                Assert.That(ex.Name, Is.EqualTo("windowName"));
            }
        }
        #endregion
    }
}