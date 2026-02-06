#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using ViewModelLib;
using WpfApp.State;

namespace WpfApp.ViewModels;

public class GridRowDuplicateBarcodeValidationRule : ValidationRule
{
    public override ValidationResult Validate(
        object value,
        System.Globalization.CultureInfo cultureInfo
    )
    {
        BindingGroup group = (BindingGroup)value;
        StringBuilder sb = null!;
        GridRecord record = null!;

        if (group.Items.Count > 0)
        {
            record = group.Items[0] as GridRecord ?? new GridRecord();
        }

        if (record != null)
        {
            var state = IoC.GetInstance<SharedState>();
            if (state == null)
            {
                return new ValidationResult(false, "shared state not found");
            }

            if (state.ProcessingDataList != null)
            {
                sb = new StringBuilder();
                using (var enumerator = state.ProcessingDataList.GetEnumerator())
                {
                    // check all records in the list before this one
                    while (enumerator.MoveNext() && !enumerator.Current.Equals(record.Data))
                    {
                        if (enumerator.Current.Barcode.Equals(record.Barcode))
                        {
                            ProcessingDataValidation.SetDuplicateBarcode(record.Data);
                            if (!sb.ToString().Contains(record.Data.Status))
                            {
                                sb.Append(
                                    (sb.Length != 0 ? ", " : string.Empty) + record.Data.Status
                                );
                            }
                        }
                    }
                    // check all records in the list after this one
                    // done by the GridRecordCollection itself
                }
            }

            record.NotifyView();
        }

        if (sb != null && sb.Length > 0)
        {
            return new ValidationResult(false, sb.ToString());
        }

        if (
            record != null
            && record.Status.Equals(ProcessingDataValidation.STATUS_DUPLICATE_BARCODE)
        )
        {
            ProcessingDataValidation.AssumeValidRecord(record.Data);
            record.NotifyView();
        }

        return ValidationResult.ValidResult;
    }
}
