#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Text;
using System.Windows;
using ViewModelLib;
using ViewModelLib.Messaging;
using WpfApp.State;

namespace WpfApp.ActionResults;

[Export(typeof(ExportResult))]
public class ExportResult : IResult
{
    [Import]
    public SharedState State { get; set; } = null!;

    [Import]
    public IMessageBroker Broker { get; set; } = null!;

    #region IResult Members

    public void Execute()
    {
        // export valid processing data
        Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.FileName =
            "WeighStation_"
            + State.Mode.ToString()
            + "_Export_"
            + DateTime.Now.ToString("yyyyMMddHHmm");
        dialog.DefaultExt = ".csv";
        dialog.Filter = "Comma Separated Values (.csv)|*.csv";
        bool? result = dialog.ShowDialog();

        if (result.HasValue && result == true)
        {
            try
            {
                var filename = dialog.FileName;
                System.Diagnostics.Trace.WriteLine("file: " + filename);
                var validRecords = State.ProcessingDataList.Where(
                    (p) => p.Processing.Equals(ProcessingDataValidation.PROCESSING_PROCESS)
                );

                if (State.Mode == ModeEnum.Dilute)
                {
                    validRecords = State.ProcessingDataList;
                }

                var exportdata = RenderExportData(State, validRecords);
                System.Diagnostics.Trace.WriteLine(exportdata);
                System.IO.File.WriteAllText(filename, exportdata, Encoding.Default);

                var notification = new MenuViewModelState();
                notification.SetNotification(System.IO.Path.GetFileName(filename) + "  OK");
                Broker.Send(new GenericMessage<MenuViewModelState>(notification));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                var notification = new MenuViewModelState();
                notification.SetNotification(ex.Message);
                Broker.Send(new GenericMessage<MenuViewModelState>(notification));
            }
        }
        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };

    #endregion

    public static string RenderExportData(SharedState state, IEnumerable<ProcessingData> data)
    {
        return state.Mode switch
        {
            ModeEnum.Tare => CsvFormatHelper.RenderTaraExport(data),
            ModeEnum.WeighSolid => CsvFormatHelper.RenderWeighSolidExport(data),
            ModeEnum.Dilute => CsvFormatHelper.RenderDiluteExport(data, state.SampleInfoList),
            _ => string.Empty,
        };
    }
}
