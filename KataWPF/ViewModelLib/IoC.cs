#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace ViewModelLib;

public static class IoC
{
    private static CompositionContainer compositionContainer = null!;

    public static void InitializeWithMef()
    {
        System.Diagnostics.Trace.WriteLine(
            "calling assembly: " + Assembly.GetCallingAssembly().FullName
        );
        System.Diagnostics.Trace.WriteLine(
            "executing assembly: " + Assembly.GetExecutingAssembly().FullName
        );
        AggregateCatalog catalog = new AggregateCatalog();
        //catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly())); // DO NOT INCLUDE
        catalog.Catalogs.Add(
            new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
        );
        compositionContainer = new CompositionContainer(catalog);
        try
        {
            compositionContainer.ComposeParts();
        }
        catch (CompositionException cex)
        {
            System.Diagnostics.Debug.WriteLine(cex.ToString());
        }
    }

    public static T? GetInstance<T>()
    {
        try
        {
            return compositionContainer.GetExportedValue<T>();
        }
        catch (ImportCardinalityMismatchException)
        {
            System.Diagnostics.Trace.WriteLine("multiple instances");
            foreach (var value in GetExportedValues<T>())
            {
                System.Diagnostics.Trace.WriteLine(
                    value?.GetType().Name + " #hash: " + value?.GetHashCode()
                );
            }

            return GetExportedValues<T>().First();
        }
    }

    public static IEnumerable<Lazy<object, object>> GetInstances(Type exportType)
    {
        return compositionContainer.GetExports(exportType, null, string.Empty);
    }

    public static IEnumerable<T> GetExportedValues<T>()
    {
        return compositionContainer.GetExportedValues<T>();
    }
}
