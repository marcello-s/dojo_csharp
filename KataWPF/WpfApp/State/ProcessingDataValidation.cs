#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public class ProcessingDataValidation
{
    // localize resources
    public const string STATUS_SCAN_ERROR = "Scan error";
    public const string STATUS_TARA_NEGATIVE = "Tara negative";
    public const string STATUS_TARA_ZERO = "Tara zero";
    public const string STATUS_WEIGHT_LT_TARA = "Weight less than Tara";
    public const string STATUS_WEIGHT_EQ_TARA = "Weight equal Tara";
    public const string STATUS_CxMr_NEGATIV = "Molarity negativ";
    public const string STATUS_Cxmgml_NEGATIV = "Cx negativ";
    public const string STATUS_Cx_ZERO = "Cx zero";
    public const string STATUS_MW_NEGATIV = "Molar weight negativ";
    public const string STATUS_MW_ZERO = "Molar weight zero";
    public const string STATUS_DUPLICATE_BARCODE = "Duplicate barcode";
    public const string STATUS_VOLUME_TOO_LOW = "Volume too low";
    public const string STATUS_VOLUME_TOO_HIGH = "Volume too high";
    public const string STATUS_OK = "OK";
    public const string PROCESSING_PROCESS = "Process";
    public const string PROCESSING_IGNORED = "Ignored";

    public static void AssumeValidRecord(ProcessingData record)
    {
        record.Status = STATUS_OK;
        record.Processing = PROCESSING_PROCESS;
    }

    public static void ValidateBarcode(ProcessingData record)
    {
        if (string.IsNullOrEmpty(record.Barcode) || record.Barcode.Equals("***"))
        {
            record.Status = STATUS_SCAN_ERROR;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void SetDuplicateBarcode(ProcessingData record)
    {
        record.Status = STATUS_DUPLICATE_BARCODE;
        record.Processing = PROCESSING_IGNORED;
    }

    public static void ValidateRegisterTara(ProcessingData record)
    {
        if (record.Tara < 0)
        {
            record.Status = STATUS_TARA_NEGATIVE;
        }

        if (record.Tara == 0)
        {
            record.Status = STATUS_TARA_ZERO;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateRegisterWeight(ProcessingData record)
    {
        if (record.SolidWeight < record.Tara)
        {
            record.Status = STATUS_WEIGHT_LT_TARA;
        }

        if (record.SolidWeight == record.Tara)
        {
            record.Status = STATUS_WEIGHT_EQ_TARA;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateTargetConcentration_mMolLtZero(ProcessingData record)
    {
        if (record.TargetConcentration_mMol < 0)
        {
            record.Status = STATUS_CxMr_NEGATIV;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateTargetConcentration_mMol(ProcessingData record)
    {
        if (record.TargetConcentration_mMol > 0.0 && record.CompoundMolarWeight < 0.0)
        {
            record.Status = STATUS_MW_NEGATIV;
        }

        if (record.TargetConcentration_mMol > 0.0 && record.CompoundMolarWeight == 0.0)
        {
            record.Status = STATUS_MW_ZERO;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateCompoundMolarWeight(ProcessingData record)
    {
        if (record.CompoundMolarWeight < 0.0)
        {
            record.Status = STATUS_MW_NEGATIV;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateTargetConcentration_mgmlLtZero(ProcessingData record)
    {
        if (record.TargetConcentration_mgml < 0)
        {
            record.Status = STATUS_Cxmgml_NEGATIV;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateEitherTargetConcentrationNotZero(ProcessingData record)
    {
        if (record.TargetConcentration_mMol == 0.0 && record.TargetConcentration_mgml == 0.0)
        {
            record.Status = STATUS_Cx_ZERO;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }

    public static void ValidateVolumeRange(
        ProcessingData record,
        double lowVolume,
        double highVolume
    )
    {
        var volume = record.Volume_l * Math.Pow(10.0, 6);
        if (volume < lowVolume)
        {
            record.Status = STATUS_VOLUME_TOO_LOW;
        }

        if (volume > highVolume)
        {
            record.Status = STATUS_VOLUME_TOO_HIGH;
        }

        if (!record.Status.Equals(STATUS_OK))
        {
            record.Processing = PROCESSING_IGNORED;
        }
    }
}
