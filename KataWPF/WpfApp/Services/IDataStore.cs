#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.Services;

public interface IDataStore
{
    string StoreLocation { set; }
    void Save(object item);
    object? Load(object key);
}
