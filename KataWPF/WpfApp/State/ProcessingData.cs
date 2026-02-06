#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public class ProcessingData
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;

    // weight unit: mg
    public double Tara { get; set; }
    public double SolidWeight { get; set; }

    public double TargetConcentration_mMol { get; set; }
    public double TargetConcentration_mgml { get; set; }
    public double CompoundMolarWeight { get; set; }
    public double Volume_l { get; set; }
    public string CompoundId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string Processing { get; set; } = string.Empty;

    public static void CalculateVolumeFromCMr(ProcessingData record)
    {
        record.Volume_l =
            1.0
            / record.TargetConcentration_mMol
            * (record.SolidWeight - record.Tara)
            / record.CompoundMolarWeight;
        if (record.CompoundMolarWeight > 0.0)
        {
            CalculateCmgmlFromVolume(record);
        }

        System.Diagnostics.Trace.WriteLine("volume[l]:" + record.Volume_l);
    }

    public static void CalculateCMrFromVolume(ProcessingData record)
    {
        record.TargetConcentration_mMol =
            1.0 / record.Volume_l * (record.SolidWeight - record.Tara) / record.CompoundMolarWeight;
    }

    public static void CalculateVolumeFromCmgml(ProcessingData record)
    {
        record.Volume_l =
            0.001 * (record.SolidWeight - record.Tara) / record.TargetConcentration_mgml;
        if (record.CompoundMolarWeight > 0.0)
        {
            CalculateCMrFromVolume(record);
        }

        System.Diagnostics.Trace.WriteLine("volume[l]:" + record.Volume_l);
    }

    public static void CalculateCmgmlFromVolume(ProcessingData record)
    {
        record.TargetConcentration_mgml =
            (record.SolidWeight - record.Tara) / record.Volume_l * 0.001;
    }
}
