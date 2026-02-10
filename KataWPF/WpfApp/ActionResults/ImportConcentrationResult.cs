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

[Export(typeof(ImportConcentrationResult))]
public class ImportConcentrationResult : IResult
{
    [Import]
    public SharedState State { get; set; } = null!;

    [Import]
    public IMessageBroker Broker { get; set; } = null!;

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

                var records = CsvFormatHelper.ParseDiluteImport(text);
                // merge with list
                foreach (var record in records)
                {
                    try
                    {
                        ProcessingData listrecord = null!;
                        if (State.ProcessingDataList.Any((r) => r.Barcode.Equals(record.Barcode)))
                        {
                            listrecord = State.ProcessingDataList.Single(
                                (r) => r.Barcode.Equals(record.Barcode)
                            );
                        }

                        if (listrecord != null)
                        {
                            if (record.TargetConcentration_mMol > 0.0)
                            {
                                listrecord.TargetConcentration_mMol =
                                    record.TargetConcentration_mMol;
                            }

                            if (record.CompoundMolarWeight > 0.0)
                            {
                                listrecord.CompoundMolarWeight = record.CompoundMolarWeight;
                            }

                            if (record.TargetConcentration_mgml > 0.0)
                            {
                                listrecord.TargetConcentration_mgml =
                                    record.TargetConcentration_mgml;
                            }

                            listrecord.CompoundId = record.CompoundId;

                            ProcessingDataValidation.AssumeValidRecord(listrecord);
                            ProcessingDataValidation.ValidateBarcode(record);
                            ProcessingDataValidation.ValidateRegisterTara(record);
                            ProcessingDataValidation.ValidateRegisterWeight(record);
                            ProcessingDataValidation.ValidateCompoundMolarWeight(listrecord);
                            ProcessingDataValidation.ValidateTargetConcentration_mMolLtZero(
                                listrecord
                            );
                            ProcessingDataValidation.ValidateTargetConcentration_mMol(listrecord);
                            if (
                                listrecord.Processing == ProcessingDataValidation.PROCESSING_PROCESS
                            )
                            {
                                ProcessingData.CalculateVolumeFromCMr(listrecord);
                            }

                            ProcessingDataValidation.ValidateTargetConcentration_mgmlLtZero(
                                listrecord
                            );

                            if (
                                listrecord.Processing == ProcessingDataValidation.PROCESSING_PROCESS
                            )
                            {
                                ProcessingData.CalculateVolumeFromCmgml(listrecord);
                            }

                            ProcessingDataValidation.ValidateEitherTargetConcentrationNotZero(
                                listrecord
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                Broker.Send(new NotificationMessage(SharedState.UpdatedMessage));

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
}
