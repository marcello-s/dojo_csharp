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

[Export(typeof(ImportResult))]
public class ImportResult : IResult
{
    [Import]
    public IMessageBroker Broker { get; set; } = null!;

    [Import]
    public SharedState State { get; set; } = null!;

    // [Import]
    // public IDataStoreService DataStore { get; set; }

    public void Execute()
    {
        // import processing data
        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.DefaultExt = ".csv";
        dialog.Filter = "Comma Separated Values (.csv)|*.csv";
        bool? result = dialog.ShowDialog();
        if (result.HasValue && result == true)
        {
            try
            {
                var filename = dialog.FileName;
                System.Diagnostics.Trace.WriteLine("file: " + filename);
                var text = System.IO.File.ReadAllText(filename, Encoding.Default);
                System.Diagnostics.Trace.WriteLine(text);
                // var records = ParseImportData(State.Mode, text);
                // foreach (var record in records)
                // {
                //    DataStore.Wrapper.Save(DataStoreMapper.MapToDatastore(record));
                // }

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

    /*
    public static IList<ProcessingData> ParseImportData(ModeEnum mode, string text)
    {
        return mode switch
        {
            ModeEnum.WeighSolid => (IList<ProcessingData>)CsvFormatHelper.ParseTaraImport(text),
            ModeEnum.Dilute => (IList<ProcessingData>)CsvFormatHelper.ParseWeighSolidImport(text),
            _ => new List<ProcessingData>(),
        };
    }
    */
}
