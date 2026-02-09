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

namespace WpfApp.ViewModels;

[Export(typeof(WeighSolidProcessingDetailsViewModel))]
[method: ImportingConstructor]
public class WeighSolidProcessingDetailsViewModel(SharedState state) : ViewModelBase, IScreen
{
    private string title = "WeighSolid Processing Details ViewModel";
    private GridRecordCollection recordCollection = null!;

    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            NotifyOfPropertyChange(() => Title);
        }
    }

    public GridRecordCollection GetWeighSolidDetails
    {
        get { return recordCollection; }
    }

    public void Activate()
    {
        if (state.ProcessingDataList != null)
        {
            GridHelper.SetupGridData(state.ProcessingDataList, out recordCollection);
        }
    }

    public void Deactivate() { }

    public bool CanClose()
    {
        return true;
    }
}
