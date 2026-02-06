#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using WpfApp.State;

namespace WpfApp.ViewModels;

public class GridHelper
{
    internal static void SetupGridData(
        IEnumerable<ProcessingData> dataList,
        out GridRecordCollection recordCollection
    )
    {
        recordCollection = null!;
        recordCollection = new GridRecordCollection();
        recordCollection.Clear();

        foreach (var dataItem in dataList)
        {
            recordCollection.Add(new GridRecord(dataItem));
        }
    }
}
