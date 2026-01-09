#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMicrokernel;

[TestFixture]
public class CyclicDependencyCheckerTests
{
    private Bindings bindings = null!;
    private CyclicDependencyChecker cyclicDependencyChecker = null!;

    [SetUp]
    public void Setup()
    {
        bindings = new Bindings();
        cyclicDependencyChecker = new CyclicDependencyChecker(bindings);
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(cyclicDependencyChecker, Is.InstanceOf<CyclicDependencyChecker>());
    }

    [Test]
    public void CheckType_WithCyclicDependency_ThrowsException()
    {
        bindings.Add(typeof(INeedYou), typeof(NeedYou));
        bindings.Add(typeof(IAndYouTogether), typeof(AndYouTogether));

        Assert.Throws<ArgumentException>(() => cyclicDependencyChecker.CheckType(typeof(INeedYou)));
    }

    [Test]
    public void CheckType_WithDirectDependency()
    {
        bindings.Add(typeof(IDateTimeProvider), typeof(DateTimeProvider));
        bindings.Add(typeof(ISystem), typeof(MySystem));
        bindings.Add(typeof(IFile), typeof(MyFile));
        cyclicDependencyChecker.CheckType(typeof(ISystem));
        Assert.Pass();
    }
}

public interface INeedYou
{
    string You();
}

public class NeedYou : INeedYou
{
    private IAndYouTogether _andYouTogether;

    public NeedYou(IAndYouTogether andYouTogether)
    {
        _andYouTogether = andYouTogether;
    }

    public string You()
    {
        return "Hello You!";
    }
}

public interface IAndYouTogether
{
    string Me();
}

public class AndYouTogether : IAndYouTogether
{
    private INeedYou _needYou;

    public AndYouTogether(INeedYou needYou)
    {
        _needYou = needYou;
    }

    public string Me()
    {
        return "Me";
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

public interface IFile
{
    bool Exists(string path);
}

class MyFile : IFile
{
    public bool Exists(string path)
    {
        return System.IO.File.Exists(path);
    }
}

public interface ISystem
{
    DateTime TimeNow();
    bool FileExists(string path);
}

class MySystem : ISystem
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFile _file;

    public MySystem(IDateTimeProvider dateTimeProvider, IFile file)
    {
        _dateTimeProvider = dateTimeProvider;
        _file = file;
    }

    public DateTime TimeNow()
    {
        return _dateTimeProvider.Now();
    }

    public bool FileExists(string path)
    {
        return _file.Exists(path);
    }
}
