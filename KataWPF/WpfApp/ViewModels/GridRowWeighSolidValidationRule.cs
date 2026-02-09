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

public class GridRowWeighSolidValidationRule : ValidationRule
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
            // validate record integrity
            ProcessingDataValidation.AssumeValidRecord(record.Data);
            sb = new StringBuilder();

            // barcode
            ProcessingDataValidation.ValidateBarcode(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            // tara
            ProcessingDataValidation.ValidateRegisterTara(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            // solid weight
            ProcessingDataValidation.ValidateRegisterWeight(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            record.WeighSolidNotifyView();
        }

        if (sb != null && sb.Length > 0)
        {
            return new ValidationResult(false, sb.ToString());
        }

        return ValidationResult.ValidResult;
    }
}
