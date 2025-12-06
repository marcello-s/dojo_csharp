#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMicrokernel;

[TestFixture]
public class BindingsTests
{
    private Bindings bindings = null!;

    [SetUp]
    public void Setup()
    {
        bindings = new Bindings();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(bindings, Is.InstanceOf<Bindings>());
    }

    [Test]
    public void Add_WithAbstractTypeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => bindings.Add(null, typeof(TestImplementation)));
    }

    [Test]
    public void Add_WithImplementationTypeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => bindings.Add(typeof(ITestInterface), null));
    }

    [Test]
    public void Add_WithAbstractTypeAndImplementationTypeAreSame_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            bindings.Add(typeof(ITestInterface), typeof(ITestInterface))
        );
    }

    [Test]
    public void Add_WithAbstractTypeAndImplementationTypeIsNotAssignable_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            bindings.Add(typeof(ITestInterface), typeof(ConcreteClass))
        );
    }

    [Test]
    public void Add_WithAbstractTypeAndImplementationTypeSwapped_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            bindings.Add(typeof(TestImplementation), typeof(ITestInterface))
        );
    }

    [Test]
    public void Add_WithInterfaceTypeAndImplementation_ReturnsCorrectType()
    {
        bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        var implementationTypes = bindings.GetImplementationTypes(typeof(ITestInterface));
        Assert.That(implementationTypes.Count(), Is.EqualTo(1));
        Assert.That(implementationTypes.First(), Is.EqualTo(typeof(TestImplementation)));
    }

    [Test]
    public void Add_WithAbstractTypeAndConcreteType_ReturnsCorrectType()
    {
        bindings.Add(typeof(AbstractBase), typeof(ConcreteClass));
        var implementationTypes = bindings.GetImplementationTypes(typeof(AbstractBase));
        Assert.That(implementationTypes.Count(), Is.EqualTo(1));
        Assert.That(implementationTypes.First(), Is.EqualTo(typeof(ConcreteClass)));
    }

    [Test]
    public void Add_WithInterfaceTypeAndImplementationTwice_ReturnsCorrectType()
    {
        bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        var implementationTypes = bindings.GetImplementationTypes(typeof(ITestInterface));
        Assert.That(implementationTypes.Count(), Is.EqualTo(1));
        Assert.That(implementationTypes.First(), Is.EqualTo(typeof(TestImplementation)));
    }

    [Test]
    public void Add_WithInterfaceAndTwoImplementations_ReturnsCorrectTypes()
    {
        bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        bindings.Add(typeof(ITestInterface), typeof(ConcreteTestImplementation));
        var implementationTypes = bindings.GetImplementationTypes(typeof(ITestInterface));
        Assert.That(implementationTypes.Count(), Is.EqualTo(2));
        Assert.That(implementationTypes.ElementAt(0), Is.EqualTo(typeof(TestImplementation)));
        Assert.That(
            implementationTypes.ElementAt(1),
            Is.EqualTo(typeof(ConcreteTestImplementation))
        );
    }

    [Test]
    public void Add_WithDerivedType_ReturnsCorrectType()
    {
        bindings.Add(typeof(ITestInterface), typeof(DerivedConcreteTestImplementation));
        var implementationTypes = bindings.GetImplementationTypes(typeof(ITestInterface));
        Assert.That(implementationTypes.Count(), Is.EqualTo(1));
        Assert.That(
            implementationTypes.First(),
            Is.EqualTo(typeof(DerivedConcreteTestImplementation))
        );
    }

    [Test]
    public void Add_WithAbstractClassAsImplementation_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            bindings.Add(typeof(ITestInterface), typeof(AbstractTestInterface))
        );
    }

    [Test]
    public void Add_WithInterfaceAsImplementation_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            bindings.Add(typeof(ITestInterface), typeof(ICombinedInterface))
        );
    }

    [Test]
    public void UpdateBinding_WithModifiedBinding()
    {
        var binding = bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        var newBinding = new Binding(binding);
        newBinding.ScopeHashCode = 1234;
        bindings.UpdateBinding(binding, newBinding);
        Assert.Pass();
    }

    [Test]
    public void GetBindingForImplementationType_CorrectBinding()
    {
        var binding1 = bindings.Add(typeof(ITestInterface), typeof(TestImplementation));
        var binding2 = bindings.Add(typeof(ITestInterface), typeof(ConcreteTestImplementation));
        var binding3 = bindings.GetBindingForImplementationType(typeof(TestImplementation));
        Assert.That(binding3, Is.EqualTo(binding1));
        var binding4 = bindings.GetBindingForImplementationType(typeof(ConcreteTestImplementation));
        Assert.That(binding4, Is.EqualTo(binding2));
    }
}

public interface ITestInterface
{
    string Hello();
}

public abstract class AbstractBase
{
    public abstract string Hello();
}

public class TestImplementation : ITestInterface
{
    public string Hello()
    {
        return "Hello World!";
    }
}

public class ConcreteClass : AbstractBase
{
    public override string Hello()
    {
        return "Hello World!";
    }
}

public class ConcreteTestImplementation : AbstractBase, ITestInterface
{
    public override string Hello()
    {
        return "Hello World!";
    }
}

public class DerivedConcreteTestImplementation : ConcreteTestImplementation { }

public abstract class AbstractTestInterface : ITestInterface
{
    public virtual string Hello()
    {
        throw new NotImplementedException();
    }
}

public interface ICombinedInterface : ITestInterface
{
    string World();
}
