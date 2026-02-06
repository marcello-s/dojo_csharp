#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;

namespace WpfApp.State;

[Export(typeof(SharedState))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class SharedState
{
    public const string NOTIFICATION_SHAREDSTATEUPDATE = "shared state update";
    public const double LOW_VOLUME = 5.0;
    public static bool IsCommandResponseNeeded { get; set; }

    public bool AllTubesSelected { get; set; }
    public int RangeTubesLowerValue { get; set; }
    public int RangeTubesUpperValue { get; set; }

    public IList<ProcessingData> ProcessingDataList { get; set; } =
        new List<ProcessingData>
        {
            new ProcessingData
            {
                Id = 1,
                Barcode = "0001",
                Tara = 3,
                SolidWeight = 10.0,
                Processing = "OK",
            },
            new ProcessingData
            {
                Id = 2,
                Barcode = "0002",
                Tara = 3,
                SolidWeight = 15.0,
                Processing = "OK",
            },
            new ProcessingData
            {
                Id = 3,
                Barcode = "0003",
                Tara = 3,
                SolidWeight = 8.0,
                Processing = "OK",
            },
            new ProcessingData
            {
                Id = 4,
                Barcode = "0004",
                Tara = 3,
                SolidWeight = 12.0,
                Processing = "OK",
            },
            new ProcessingData
            {
                Id = 5,
                Barcode = "0005",
                Tara = 3,
                SolidWeight = 20.0,
                Processing = "OK",
            },
        };
}
