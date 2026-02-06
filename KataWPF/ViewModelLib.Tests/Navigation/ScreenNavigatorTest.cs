#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.CompilerServices;
using NUnit.Framework;
using ViewModelLib;
using ViewModelLib.Navigation;

namespace ViewModelLib.Test.Navigation;

[TestFixture]
public class ScreenNavigatorTest
{
    private IScreenNavigator navigator = null!;

    [SetUp]
    public void Setup()
    {
        navigator = new ScreenNavigator();
    }

    [Test]
    public void AddTest()
    {
        INavigatable myScreen1 = new NavigationItem("myScreen1");
        navigator.Add(myScreen1);
        INavigatable myScreen2 = new NavigationItem("myScreen2");
        navigator.Add(myScreen2);
        Assert.That(navigator.Current, Is.EqualTo(myScreen1));
    }

    [Test]
    public void RemoveTest()
    {
        INavigatable myScreen1 = new NavigationItem("myScreen1");
        navigator.Add(myScreen1);
        INavigatable myScreen2 = new NavigationItem("myScreen2");
        navigator.Add(myScreen2);
        Assert.That(navigator.Current, Is.EqualTo(myScreen1));

        navigator.Remove(myScreen1);
        navigator.Remove(myScreen2);
        Assert.That(navigator.Current, Is.Null);
    }

    [Test]
    public void NavigateToTest()
    {
        INavigatable myScreen1 = new NavigationItem(typeof(string));
        navigator.Add(myScreen1);
        INavigatable myScreen2 = new NavigationItem(typeof(int));
        navigator.Add(myScreen2);

        navigator.NavigateTo(typeof(int));
        Assert.That(navigator.Current, Is.Not.Null);
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void ForwardAndBackTest()
    {
        INavigatable myItem1 = new NavigationItem(typeof(string));
        INavigatable myItem2 = new NavigationItem(typeof(int));
        INavigatable myItem3 = new NavigationItem(typeof(bool));
        myItem1.Next = myItem2;
        myItem2.Previous = myItem1;
        myItem2.Next = myItem3;
        myItem3.Previous = myItem2;
        navigator.Add(myItem1);
        navigator.Add(myItem2);
        navigator.Add(myItem3);

        navigator.NavigateTo(myItem1.Target!);
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(string)));
        // forward
        INavigatable item = navigator.Forward()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(int)));
        item = navigator.Forward()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(bool)));
        item = navigator.Forward()!;
        Assert.That(item, Is.Null);
        // back
        item = navigator.Back()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(int)));
        item = navigator.Back()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(string)));
        item = navigator.Back()!;
        Assert.That(item, Is.Null);
    }

    [Test]
    public void ForwardAndBackNotActiveTest()
    {
        INavigatable myItem1 = new NavigationItem(typeof(string));
        INavigatable myItem2 = new NavigationItem(typeof(int));
        INavigatable myItem3 = new NavigationItem(typeof(bool));
        myItem1.Next = myItem2;
        myItem2.Previous = myItem1;
        myItem2.Next = myItem3;
        myItem3.Previous = myItem2;
        navigator.Add(myItem1);
        navigator.Add(myItem2);
        navigator.Add(myItem3);

        // not active
        myItem2.IsActive = false;

        navigator.NavigateTo(myItem1.Target!);
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(string)));
        // forward
        INavigatable item = navigator.Forward()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(bool)));
        item = navigator.Forward()!;
        Assert.That(item, Is.Null);
        // back
        item = navigator.Back()!;
        Assert.That(navigator.Current.Target, Is.EqualTo(typeof(string)));
        item = navigator.Back()!;
        Assert.That(item, Is.Null);
    }
}
