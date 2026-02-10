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

[Export(typeof(SelectRangeResult))]
public class SelectRangeResult : IResult
{
    [Import]
    public SharedState State { get; set; } = null!;

    public bool AllTubes { get; set; }
    public double TubeRangeLowerValue { get; set; }
    public double TubeRangeUpperValue { get; set; }
    public double TubeRangeMinimum { get; set; }
    public double TubeRangeMaximum { get; set; }

    public SelectRangeResult With(
        bool allTubes,
        double tubeRangeLowerValue,
        double tubeRangeUpperValue,
        double tubeRangeMinimum,
        double tubeRangeMaximum
    )
    {
        AllTubes = allTubes;
        TubeRangeLowerValue = tubeRangeLowerValue;
        TubeRangeUpperValue = tubeRangeUpperValue;
        TubeRangeMinimum = tubeRangeMinimum;
        TubeRangeMaximum = tubeRangeMaximum;
        return this;
    }

    public void Execute()
    {
        // save values to state
        State.AllTubesSelected = AllTubes;
        State.RangeTubesLowerValue = (int)TubeRangeLowerValue;
        State.RangeTubesUpperValue = (int)TubeRangeUpperValue;

        int rangeLower,
            rangeUpper;
        if (State.AllTubesSelected)
        {
            rangeLower = (int)TubeRangeMinimum;
            rangeUpper = (int)TubeRangeMaximum;
        }
        else
        {
            rangeLower = State.RangeTubesLowerValue;
            rangeUpper = State.RangeTubesUpperValue;
        }

        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
