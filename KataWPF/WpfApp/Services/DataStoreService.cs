#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Configuration;

namespace WpfApp.Services;

[Export(typeof(IDataStoreService))]
public class DataStoreService : IDataStoreService
{
    private static IDataStore dataStore = new XmlDataStore();
    private const string Filename = "DataStore.xml";
    private const string DataPath = @"C:\temp\";

    public DataStoreService()
    {
        try
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyLocation);
            var shimModeString = string.Empty;
            if (config.AppSettings.Settings.AllKeys.Contains("ShimMode"))
            {
                shimModeString = config.AppSettings.Settings["ShimMode"].Value;
            }

            bool shimMode = false;

            // path probing
            var appFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            );
            var path = System.IO.Path.Combine(appFolder, DataPath, Filename);
            System.Diagnostics.Trace.WriteLine("data store path probing: " + path);
            if (!System.IO.File.Exists(path))
            {
                path = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(assemblyLocation)!,
                    Filename
                );
                System.Diagnostics.Trace.WriteLine("data store path probing: " + path);
                if (!System.IO.File.Exists(path))
                {
                    path = string.Empty;
                }
            }

            if (
                bool.TryParse(shimModeString, out shimMode)
                && shimMode
                && string.IsNullOrEmpty(path)
            )
            {
                dataStore = new DataStoreShimWrapper();
            }
            else
            {
                dataStore = new XmlDataStore();
            }
            dataStore.StoreLocation = path;
            System.Diagnostics.Trace.WriteLine(
                "datastore service type: " + dataStore.GetType().Name
            );
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(
                "datastore service failed to start up: " + ex.Message
            );
        }
    }

    public IDataStore Wrapper
    {
        get { return dataStore; }
    }

    internal class DataStoreShimWrapper : IDataStore
    {
        public string StoreLocation
        {
            set { System.Diagnostics.Trace.WriteLine("store location: " + value); }
        }

        public void Save(object item)
        {
            System.Diagnostics.Trace.WriteLine("save item : " + item.GetType().Name);
        }

        public object Load(object key)
        {
            throw new NotImplementedException();
        }
    }
}
