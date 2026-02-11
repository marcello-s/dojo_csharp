#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;
using ViewModelLib;
using WpfApp.Services;
using WpfApp.State;

namespace WpfApp.ActionResults;

[Export(typeof(CommitValidProcessingDataResult))]
public class CommitValidProcessingDataResult : IResult
{
    [Import]
    public SharedState State { get; set; } = null!;

    [Import]
    public IDataStoreService DataStore { get; set; } = null!;

    public void Execute()
    {
        // commit valid processing data
        if (State.ProcessingDataList != null)
        {
            var validRecords = State.ProcessingDataList.Where(
                (p) => p.Processing.Equals(ProcessingDataValidation.PROCESSING_PROCESS)
            );

            foreach (var record in validRecords)
            {
                DataStore.Wrapper.Save(DataStoreMapper.MapToDatastore(record));
            }

            // clear list
            State.ProcessingDataList.Clear();
        }

        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
