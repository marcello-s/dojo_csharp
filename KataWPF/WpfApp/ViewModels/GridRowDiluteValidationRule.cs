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

public class GridRowDiluteValidationRule : ValidationRule
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

            // molar weight
            ProcessingDataValidation.ValidateCompoundMolarWeight(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            // TargetConcentration mMol
            ProcessingDataValidation.ValidateTargetConcentration_mMolLtZero(record.Data);
            ProcessingDataValidation.ValidateTargetConcentration_mMol(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }
            else
            {
                ProcessingData.CalculateVolumeFromCMr(record.Data);
            }

            // TargetConcentration mg/ml
            ProcessingDataValidation.ValidateTargetConcentration_mgmlLtZero(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }
            else
                ProcessingData.CalculateVolumeFromCmgml(record.Data);

            // Volume range
            var state = IoC.GetInstance<SharedState>();
            ProcessingDataValidation.ValidateVolumeRange(
                record.Data,
                SharedState.LowVolume,
                state!.MaxVolume
            );

            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            // TargetConcentration not zero
            ProcessingDataValidation.ValidateEitherTargetConcentrationNotZero(record.Data);
            if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
            {
                if (!sb.ToString().Contains(record.Data.Status))
                {
                    sb.Append((sb.Length != 0 ? ", " : string.Empty) + record.Data.Status);
                }
            }

            record.DiluteNotifyView();
        }

        if (sb != null && sb.Length > 0)
        {
            return new ValidationResult(false, sb.ToString());
        }

        return ValidationResult.ValidResult;
    }
}
