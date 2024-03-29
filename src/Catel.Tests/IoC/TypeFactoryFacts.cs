﻿namespace Catel.Tests.IoC
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Data;
    using NUnit.Framework;

    public class TypeFactoryFacts
    {
        public class StaticCtorClass
        {
            static StaticCtorClass()
            {

            }

            public StaticCtorClass()
            {

            }
        }

        public interface IDummyDependency
        {
            string Value { get; set; }
        }

        public class DummyDependency : IDummyDependency
        {
            public string Value { get; set; }
        }

        public class DependencyInjectionTestClass : INeedCustomInitialization
        {
            public DependencyInjectionTestClass()
            {
                UsedDefaultConstructor = true;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry)
            {
                IniEntry = iniEntry;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry, int intValue)
                : this(iniEntry)
            {
                IntValue = intValue;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry, int intValue, string stringValue)
                : this(iniEntry, intValue)
            {
                StringValue = stringValue;
            }

            public bool UsedDefaultConstructor { get; private set; }

            public IniEntry IniEntry { get; private set; }

            public int IntValue { get; private set; }

            public string StringValue { get; private set; }

            public bool HasCalledCustomInitialization { get; private set; }

            void INeedCustomInitialization.Initialize()
            {
                HasCalledCustomInitialization = true;
            }
        }

        public class AdvancedDependencyInjectionTestClass
        {
            public AdvancedDependencyInjectionTestClass(int intValue, IMessageService messageService, INavigationService navigationService)
            {
                ArgumentNullException.ThrowIfNull(messageService);
                ArgumentNullException.ThrowIfNull(navigationService);

                IntValue = intValue;
            }

            public AdvancedDependencyInjectionTestClass(int intValue, IMessageService messageService, INavigationService navigationService,
                IDummyDependency dependency)
            {
                ArgumentNullException.ThrowIfNull(messageService);
                ArgumentNullException.ThrowIfNull(navigationService);
                ArgumentNullException.ThrowIfNull(dependency);

                IntValue = intValue;
                Dependency = dependency;
            }

            public AdvancedDependencyInjectionTestClass(string stringValue, int intValue, long longValue, IMessageService messageService,
                INavigationService navigationService)
            {
                ArgumentNullException.ThrowIfNull(messageService);
                ArgumentNullException.ThrowIfNull(navigationService);

                StringValue = stringValue;
                IntValue = intValue;
                LongValue = longValue;
            }

            public AdvancedDependencyInjectionTestClass(string stringValue, int intValue, long longValue, IMessageService messageService,
                INavigationService navigationService, IDummyDependency dependency)
            {
                ArgumentNullException.ThrowIfNull(messageService);
                ArgumentNullException.ThrowIfNull(navigationService);
                ArgumentNullException.ThrowIfNull(dependency);

                StringValue = stringValue;
                IntValue = intValue;
                LongValue = longValue;
                Dependency = dependency;
            }

            public int IntValue { get; private set; }

            public string StringValue { get; private set; }

            public long LongValue { get; private set; }

            public IDummyDependency Dependency { get; private set; }
        }

        [TestFixture]
        public class TheCreateInstanceWithParametersMethod
        {
            public interface IInterface
            {
                bool SignalI { get; set; }

            }

            public class ClassA : IInterface
            {
                public bool SignalI { get; set; }
                public bool SignalA { get; set; }
            }

            public class ClassB : IInterface
            {
                public ClassB(IInterface i)
                {
                    i.SignalI = true;
                }

                public ClassB(ClassB b)
                {
                    b.SignalB = true;
                }

                public ClassB(ClassB b1, ClassB b2)
                {
                    b1.SignalB = true;
                    b2.SignalB = true;
                }

                public ClassB(ClassB b, ClassA a)
                {
                    b.SignalB = true;
                    a.SignalA = true;
                }

                public ClassB(ClassB b, IInterface i)
                {
                    b.SignalB = true;
                    i.SignalI = true;
                }

                public ClassB(IInterface i, ClassA a)
                {
                    i.SignalI = true;
                    a.SignalA = true;
                }

                public ClassB(ClassC c, ClassA a)
                {
                    c.SignalC = true;
                    a.SignalA = true;
                }

                public bool SignalB { get; set; }

                public bool SignalI { get; set; }
            }

            public class ClassC : ClassA
            {
                public bool SignalC { get; set; }
            }

            [Test]
            public void UseTheMostSpecializedConstructor()
            {
                ClassB b = new ClassB(new ClassA());

                TypeFactory.Default.CreateInstanceWithParameters(typeof(ClassB), b);

                Assert.That(b.SignalB, Is.True);
            }

            [Test]
            public void UseTheMostSpecializedConstructor2()
            {
                ClassB b1 = new ClassB(new ClassA());
                ClassB b2 = new ClassB(new ClassA());

                TypeFactory.Default.CreateInstanceWithParameters(typeof(ClassB), b1, b2);

                Assert.That(b1.SignalB, Is.True);
                Assert.That(b2.SignalB, Is.True);
            }

            [Test]
            public void UseTheMostSpecializedConstructor3()
            {
                ClassA a = new ClassA();
                ClassB b = new ClassB(a);

                TypeFactory.Default.CreateInstanceWithParameters(typeof(ClassB), b, a);

                Assert.That(b.SignalB, Is.True);
                Assert.That(a.SignalA, Is.True);
            }

            [Test]
            public void UseTheMostSpecializedConstructor4()
            {
                ClassA a = new ClassA();
                ClassC c = new ClassC();

                TypeFactory.Default.CreateInstanceWithParameters(typeof(ClassB), c, a);

                Assert.That(c.SignalC, Is.True);
                Assert.That(a.SignalA, Is.True);
            }
        }

        [TestFixture]
        public class TheCreateInstanceMethod
        {
            [TestCase]
            public void ResolvesTypeWithStaticAndNonStaticConstructorButUsesNonStatic()
            {
                Assert.That(TypeFactory.Default.CreateInstance<StaticCtorClass>(), Is.InstanceOf(typeof(StaticCtorClass)));
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                    Assert.That(instance.UsedDefaultConstructor, Is.True);
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToFirstConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(0));
                    Assert.That(instance.StringValue, Is.EqualTo(null));
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToSecondConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);
                    serviceLocator.RegisterInstance(42);

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(42));
                    Assert.That(instance.StringValue, Is.EqualTo(null));
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionUsesConstructorWithMostParametersFirst()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);
                    serviceLocator.RegisterInstance(42);
                    serviceLocator.RegisterInstance("hi there");

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(42));
                    Assert.That(instance.StringValue, Is.EqualTo("hi there"));
                }
            }

            [TestCase]
            public void CallsCustomInitializationWhenNeeded()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();
                    Assert.That(instance.HasCalledCustomInitialization, Is.True);
                }
            }

            [TestCase]
            public void AutomaticallyRegistersDependencyResolverInDependencyResolverManager()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();
                    var dependencyResolverManager = DependencyResolverManager.Default;
                    var actualDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(instance);

                    Assert.That(actualDependencyResolver, Is.EqualTo(dependencyResolver));
                }
            }

            public class X
            {
                public X(Y y) { }
            }

            public class Y
            {
                public Y(Z z) { }
            }

            public class Z
            {
                public Z(X x) { }
            }

            [TestCase]
            public void ThrowsCircularDependencyExceptionForInvalidTypeRequestPath()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

                    serviceLocator.RegisterType<X>();
                    serviceLocator.RegisterType<Y>();
                    serviceLocator.RegisterType<Z>();

                    var ex = Assert.Throws<CircularDependencyException>(() => typeFactory.CreateInstance<X>());

                    Assert.That(ex.TypePath.AllTypes.Count(), Is.EqualTo(3));
                    Assert.That(ex.TypePath.FirstType.Type, Is.EqualTo(typeof(X)));
                    Assert.That(ex.DuplicateRequestInfo.Type, Is.EqualTo(typeof(X)));
                }
            }
        }

        [TestFixture]
        public class TheCreateInstanceWithTagMethod
        {

        }

        [TestFixture]
        public class TheCreateInstanceWithAutoCompletionMethod
        {
            public class Person
            {
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            public class ClassWithDynamicConstructor
            {
                public ClassWithDynamicConstructor(dynamic person)
                {
                    IsDynamicConstructorCalled = true;
                }

                public ClassWithDynamicConstructor(Person person)
                {
                    IsTypedConstructorCalled = true;
                }

                public bool IsDynamicConstructorCalled { get; private set; }

                public bool IsTypedConstructorCalled { get; private set; }
            }

            public class ClassWithSeveralMatchesForDependencyInjection
            {
                public ClassWithSeveralMatchesForDependencyInjection(IUIVisualizerService uiVisualizerService, IMessageService messageService)
                {
                }

                public bool IsRightConstructorUsed { get; private set; }
            }

            [TestCase]
            public void CreatesTypeUsingSimpleCustomInjectionAndAutoCompletion()
            {
                var typeFactory = TypeFactory.Default;
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AdvancedDependencyInjectionTestClass>(42);

                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.IntValue, Is.EqualTo(42));
            }

            [TestCase]
            public void CreatesTypeUsingComplexCustomInjectionAndAutoCompletion()
            {
                var typeFactory = TypeFactory.Default;
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AdvancedDependencyInjectionTestClass>("string", 42, 42L);

                Assert.That(instance, Is.Not.Null);
                Assert.That(instance.StringValue, Is.EqualTo("string"));
                Assert.That(instance.IntValue, Is.EqualTo(42));
                Assert.That(instance.LongValue, Is.EqualTo(42L));
            }

            [TestCase]
            public void CreatesTypeWhenDynamicConstructorIsAvailable()
            {
                var typeFactory = TypeFactory.Default;

                var person = new Person { FirstName = "John", LastName = "Doe" };
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ClassWithDynamicConstructor>(person);

                Assert.That(instance.IsDynamicConstructorCalled, Is.False);
                Assert.That(instance.IsTypedConstructorCalled, Is.True);
            }

            [TestCase, Explicit]
            public void IfTypeFactoryIsCalledConcurrentlyItRunsFasterThanSerial()
            {
                const int itemsPerThread = 50;
                const int threadAmount = 10;

                var typeFactory = TypeFactory.Default;

                var serialStopWatch = new Stopwatch();
                serialStopWatch.Start();
                for (int i = 0; i < itemsPerThread * threadAmount; i++)
                {
                    typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeof(AdvancedDependencyInjectionTestClass), 30);
                }
                serialStopWatch.Stop();

                // Skip Thread creation in benchmark
                var paralellStopWatch = new Stopwatch();
                paralellStopWatch.Start();

                var threads = new Thread[threadAmount];
                for (int i = 0; i < threadAmount; i++)
                {
                    threads[i] = new Thread((index) =>
                    {
                        typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeof(AdvancedDependencyInjectionTestClass), 30);
                    });
                }

                for (int i = 0; i < threadAmount; i++)
                {
                    threads[i].Start(i);
                }

                for (int i = 0; i < threadAmount; i++)
                {
                    threads[i].Join();
                }
                paralellStopWatch.Stop();

                Assert.That(paralellStopWatch.ElapsedMilliseconds, Is.LessThan(serialStopWatch.ElapsedMilliseconds / 5));
            }
        }

        [TestFixture]
        public class TheCreateInstanceWithAutoCompletionWithTagMethod
        {
            [TestCase]
            public void CreatesTypeWhenDynamicConstructorIsAvailable()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var noTagDependency = new DummyDependency
                    {
                        Value = "no tag"
                    };

                    var tagDependency = new DummyDependency
                    {
                        Value = "tag"
                    };

                    serviceLocator.RegisterType<IDispatcherProviderService, DispatcherProviderService>();
                    serviceLocator.RegisterType<IDispatcherService, DispatcherService>();
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    serviceLocator.RegisterType<IUrlLocator, UrlLocator>();
                    serviceLocator.RegisterType<INavigationService, NavigationService>();
                    serviceLocator.RegisterType<INavigationRootService, NavigationRootService>();
                    serviceLocator.RegisterType<ILanguageService, LanguageService>();
                    serviceLocator.RegisterInstance<IDummyDependency>(noTagDependency);
                    serviceLocator.RegisterInstance<IDummyDependency>(tagDependency, "tag");

                    using (var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>())
                    {
                        var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletionWithTag<AdvancedDependencyInjectionTestClass>("tag", "string", 42, 42L);

                        Assert.That(instance, Is.Not.Null);
                        Assert.That(instance.StringValue, Is.EqualTo("string"));
                        Assert.That(instance.IntValue, Is.EqualTo(42));
                        Assert.That(instance.LongValue, Is.EqualTo(42L));

                        Assert.That(ReferenceEquals(tagDependency, instance.Dependency), Is.True);
                    }
                }
            }
        }
    }
}
