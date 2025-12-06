#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMicrokernel;

[TestFixture]
public class ScopeTests
{
    private Scope scope = null!;

    [SetUp]
    public void Setup()
    {
        scope = new Scope();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(scope, Is.InstanceOf<Scope>());
    }

    [Test]
    public void Add_WithTypeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => scope.Add(null, 0));
    }

    [Test]
    public void Add_WithInstanceNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => scope.Add(typeof(int), null));
    }

    [Test]
    public void Add_WithTypeAndInstance()
    {
        scope.Add(typeof(int), 0);
        Assert.Pass();
    }

    [Test]
    public void Find_WithTypeAndInstance_ReturnInstance()
    {
        scope.Add(typeof(int), 0);
        var instance = Scope.Find(scope, typeof(int));
        Assert.That(instance, Is.InstanceOf<int>());
    }

    [Test]
    public void Find_WithNoInstanceCached_ReturnsNull()
    {
        var instance = Scope.Find(scope, typeof(int));
        Assert.That(instance, Is.Null);
    }

    [Test]
    public void Find_WithChildScope_ReturnInstance()
    {
        scope.Add(typeof(int), 0);
        var childScope = new Scope(scope);
        var instance = Scope.Find(childScope, typeof(int));
        Assert.That(instance, Is.InstanceOf<int>());
    }

    [Test]
    public void Find_With3LevelsOfScope_ReturnInstance()
    {
        scope.Add(typeof(int), 0);
        var scope01 = new Scope(scope);
        scope01.Add(typeof(string), string.Empty);
        var scope02 = new Scope(scope01);
        scope02.Add(typeof(bool), true);
        var instance = Scope.Find(scope02, typeof(int));
        Assert.That(instance, Is.InstanceOf<int>());
    }
}
