﻿namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;
    using ViewModels.TestClasses;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class ViewModelCommandManagerFacts
    {
        [TestFixture]
        public class TheCreateMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullViewModel()
            {
                Assert.Throws<ArgumentNullException>(() => ViewModelCommandManager.Create(null));
            }

            [TestCase]
            public void ReturnsViewModelCommandManagerForViewModel()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.IsNotNull(viewModelCommandManager);
            }
        }

        [TestFixture]
        public class TheAddHandlerMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullHandler()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);

                Assert.Throws<ArgumentNullException>(() => viewModelCommandManager.AddHandler((Func<IViewModel, string, ICommand, object, Task>)null));
            }

            [TestCase]
            public async Task RegisteredHandlerGetsCalledAsync()
            {
                var viewModel = new TestViewModel();
                var viewModelCommandManager = ViewModelCommandManager.Create(viewModel);
                await viewModel.InitializeViewModelAsync();

                var called = false;

                viewModelCommandManager.AddHandler(async (vm, property, command, commandParameter) => called = true);
                viewModel.GenerateData.Execute();

                Assert.That(called, Is.True);
            }
        }
    }
}