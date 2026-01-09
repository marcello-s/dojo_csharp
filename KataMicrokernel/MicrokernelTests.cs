#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMicrokernel;

[TestFixture]
public class MicrokernelTests
{
    private Microkernel microkernel = null!;

    [SetUp]
    public void Setup()
    {
        microkernel = new Microkernel();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(microkernel, Is.InstanceOf<Microkernel>());
    }

    [Test]
    public void GetInstances_WithConstructorLessType_ResolveCorrectType()
    {
        microkernel.Bind<ITestInterface, ConstructorLessImplementation>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<ConstructorLessImplementation>());
    }

    [Test]
    public void GetInstances_WithInternalType_ResolveCorrectType()
    {
        microkernel.Bind<ITestInterface, InternalImplementation>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<InternalImplementation>());
    }

    [Test]
    public void GetInstances_WithDefaultValueType_ResolveCorrectType()
    {
        microkernel.Bind<ITestInterface, DefaultValueImplementation>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<DefaultValueImplementation>());
    }

    [Test]
    public void GetInstances_WithImplementationWithDependency_ResolveCorrectTypes()
    {
        microkernel.Bind<ITestInterface, ImplementationWithDependency>();
        microkernel.Bind<ITranslationService, TranslationService>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<ImplementationWithDependency>());
    }

    [Test]
    public void GetInstance_WithComplexType_ResolveCorrectTypes()
    {
        microkernel.Bind<ITestInterface, ComplexType>();
        microkernel.Bind<ITranslationService, TranslationService>();
        microkernel.Bind<IDateTimeProvider, DateTimeProvider>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<ComplexType>());
    }

    [Test]
    public void GetInstance_WithComplexTypeDependencyNotRegistered_ThrowsError()
    {
        microkernel.Bind<ITestInterface, ComplexType>();
        microkernel.Bind<ITranslationService, TranslationService>();
        var instances = microkernel.GetInstances<ITestInterface>();
        Assert.Throws<InvalidOperationException>(() => instances?.Count());
        //Assert.AreEqual(1, instances.Count());
        //Assert.IsInstanceOf(typeof(ComplexType), instances.First());
    }

    [Test]
    public void GetInstance_WithAnotherTestImplementatioIn_ResolveCorrectTypes()
    {
        microkernel.Bind<ITestInterface, ConstructorLessImplementation>();
        microkernel.Bind<ITestInterface, InternalImplementation>();
        microkernel.Bind<ITestInterface, DefaultValueImplementation>();
        microkernel.Bind<IAnotherTestInterface, AnotherTestImplementation>();
        var instances = microkernel.GetInstances<IAnotherTestInterface>();
        Assert.That(instances?.Count(), Is.EqualTo(1));
        Assert.That(instances?.First(), Is.InstanceOf<AnotherTestImplementation>());
    }

    [Test]
    public void Bind_WithTypesInSingletonScope_ReturnSingletonInstance()
    {
        microkernel.Bind<ITestInterface, ConstructorLessImplementation>().InGlobalSingletonScope();
        var instances1 = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances1?.Count(), Is.EqualTo(1));
        var instanceHashcode = instances1?.First().GetHashCode();
        var instances2 = microkernel.GetInstances<ITestInterface>();
        Assert.That(instances2?.Count(), Is.EqualTo(1));
        Assert.That(instances2?.First().GetHashCode(), Is.EqualTo(instanceHashcode));
    }

    [Test]
    public void Bind_WithCyclicDependencyGraph_ThrowsException()
    {
        microkernel.Bind<IComponentA, ComponentA>();
        microkernel.Bind<IComponentB, ComponentB>();

        Assert.Throws<ArgumentException>(() => microkernel.GetInstances<IComponentA>());
    }

    public interface ITestInterface
    {
        string HelloWorld();
    }

    public class ConstructorLessImplementation : ITestInterface
    {
        public string HelloWorld()
        {
            return "Hello World!";
        }
    }

    internal class InternalImplementation : ITestInterface
    {
        public string HelloWorld()
        {
            return "Hello World!";
        }
    }

    public class DefaultValueImplementation : ITestInterface
    {
        private readonly string _helloWorld;

        public DefaultValueImplementation(string helloWorld = "Hello World!")
        {
            _helloWorld = helloWorld;
        }

        public string HelloWorld()
        {
            return _helloWorld;
        }
    }

    public interface ITranslationService
    {
        string TranslateWordFromEnglishToGerman(string word);
    }

    public class TranslationService : ITranslationService
    {
        public string TranslateWordFromEnglishToGerman(string word)
        {
            if (word.Equals("Hello"))
            {
                return "Hallo";
            }

            return word.Equals("World") ? "Welt" : string.Empty;
        }
    }

    public class ImplementationWithDependency : ITestInterface
    {
        private readonly ITranslationService _translationService;

        public ImplementationWithDependency()
        {
            throw new NotSupportedException();
        }

        public ImplementationWithDependency(ITranslationService translationService)
        {
            if (translationService == null)
                throw new ArgumentNullException("translationService");
            _translationService = translationService;
        }

        public string HelloWorld()
        {
            const string helloWorld = "Hello World";
            var words = helloWorld
                .Split(' ')
                .ToList()
                .Select(word => _translationService.TranslateWordFromEnglishToGerman(word))
                .ToList();
            return string.Join(" ", words);
        }
    }

    public interface IDateTimeProvider
    {
        DateTime Now();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }

    public class ComplexType : ITestInterface
    {
        private readonly ITranslationService _translationService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ComplexType(
            ITranslationService translationService,
            IDateTimeProvider dateTimeProvider
        )
        {
            if (translationService == null)
            {
                throw new ArgumentNullException("translationService");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            _translationService = translationService;
            _dateTimeProvider = dateTimeProvider;
        }

        public string HelloWorld()
        {
            const string helloWorld = "Hello World";
            var words = helloWorld
                .Split(' ')
                .ToList()
                .Select(word => _translationService.TranslateWordFromEnglishToGerman(word))
                .ToList();
            return string.Join(" ", words) + " @ " + _dateTimeProvider.Now();
        }
    }

    public interface IAnotherTestInterface
    {
        void RunTestInterfaces();
    }

    public class AnotherTestImplementation : IAnotherTestInterface
    {
        private IEnumerable<ITestInterface> _testInterfaces;

        public AnotherTestImplementation(IEnumerable<ITestInterface> testInterfaces)
        {
            if (testInterfaces == null)
            {
                throw new ArgumentNullException("testInterfaces");
            }

            _testInterfaces = testInterfaces;
        }

        public void RunTestInterfaces()
        {
            foreach (var testInterface in _testInterfaces)
            {
                Console.Out.WriteLine(testInterface.HelloWorld());
            }
        }
    }

    public interface IComponentA
    {
        void AMethod();
    }

    public class ComponentA : IComponentA
    {
        private readonly IComponentB _componentB;

        public ComponentA(IComponentB componentB)
        {
            _componentB = componentB;
        }

        public void AMethod()
        {
            throw new NotImplementedException();
        }
    }

    public interface IComponentB
    {
        void BMethod();
    }

    public class ComponentB : IComponentB
    {
        private readonly IComponentA _componentA;

        public ComponentB(IComponentA componentA)
        {
            _componentA = componentA;
        }

        public void BMethod()
        {
            throw new NotImplementedException();
        }
    }
}
