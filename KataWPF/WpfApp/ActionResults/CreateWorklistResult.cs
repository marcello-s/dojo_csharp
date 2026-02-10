#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;
using ViewModelLib;
using WpfApp.State;

namespace WpfApp.ActionResults;

[Export(typeof(CreateWorklistResult))]
public class CreateWorklistResult : IResult
{
    [Import]
    public SharedState State { get; set; } = null!;

    public void Execute()
    {
        // create worklist
        if (State.Mode == ModeEnum.Dilute && State.ProcessingDataList != null)
        {
            var validRecords = State.ProcessingDataList.Where(
                (p) => p.Processing.Equals(ProcessingDataValidation.PROCESSING_PROCESS)
            );
            foreach (var record in validRecords)
            {
                System.Diagnostics.Trace.WriteLine(
                    $"worklist record: {record.Id} {record.Barcode} {record.Tara} {record.SolidWeight} {record.Processing}"
                );
            }
        }

        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
