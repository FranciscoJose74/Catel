﻿namespace Catel.Tests.MVVM.Exceptions
{
    using Catel.MVVM;

    using NUnit.Framework;

    [TestFixture]
    public class ViewModelNotRegisteredExceptionTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            try
            {
                throw new ViewModelNotRegisteredException(typeof(ViewModelBase));
            }
            catch (ViewModelNotRegisteredException ex)
            {
                Assert.That(ex.ViewModelType, Is.EqualTo(typeof(ViewModelBase)));
            }
        }
        #endregion
    }
}