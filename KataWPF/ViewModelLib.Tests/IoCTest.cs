#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;
using ViewModelLib;
using ViewModelLib.Navigation;

namespace ViewModelLib.Test;

[TestFixture]
public class IoCTest
{
    [SetUp]
    public void Setup()
    {
        IoC.InitializeWithMef();
    }

    [Ignore("for manual testing only")]
    public void GetExportedValuesTest()
    {
        var nav1 = IoC.GetExportedValues<IScreenNavigator>();
        if (nav1 != null)
        {
            Console.WriteLine(nav1.GetType().FullName);
            foreach (var item in nav1)
            {
                Console.WriteLine(item.GetType().FullName + " " + item.GetHashCode());
            }
        }

        var nav2 = IoC.GetInstance<IScreenNavigator>();
        if (nav2 != null)
        {
            Console.WriteLine(nav2.GetType().FullName + " " + nav2.GetHashCode());
        }
    }
}
