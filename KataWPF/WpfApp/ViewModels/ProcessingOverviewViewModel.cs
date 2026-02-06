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

[Export(typeof(ProcessingOverviewViewModel))]
[method: ImportingConstructor]
public class ProcessingOverviewViewModel(SharedState state) : ViewModelBase, IScreen
{
    private string title = "Processing Overview ViewModel";
    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            NotifyOfPropertyChange(() => Title);
        }
    }

    private string errorSummary = "Processing completed sucessfully";
    public string ErrorSummary
    {
        get { return errorSummary; }
        set
        {
            errorSummary = value;
            NotifyOfPropertyChange(() => ErrorSummary);
        }
    }

    private bool errorPanel = false;
    public bool ErrorPanel
    {
        get { return errorPanel; }
        set
        {
            errorPanel = value;
            NotifyOfPropertyChange(() => ErrorPanel);
        }
    }

    private int barcodeErrors = 55;
    public string BarcodeErrors
    {
        get { return barcodeErrors.ToString() + " barcode errors"; }
    }

    private int weighingErrors = 42;
    public string WeighingErrors
    {
        get { return weighingErrors.ToString() + " weighing errors"; }
    }

    public IResult View()
    {
        System.Diagnostics.Trace.WriteLine("view details");
        return null!;
    }

    public void Activate()
    {
        UpdateFigures();
        UpdateButtons();
    }

    public void Deactivate() { }

    public bool CanClose()
    {
        return true;
    }

    public void UpdateFigures()
    {
        if (state == null)
        {
            return;
        }

        NotifyOfPropertyChange(() => BarcodeErrors);
        NotifyOfPropertyChange(() => WeighingErrors);
        ErrorPanel = (barcodeErrors > 0 || weighingErrors > 0) ? true : false;
        if (!ErrorPanel)
        {
            ErrorSummary = "Processing completed sucessfully";
        }
        else
        {
            var summary = string.Empty;
            if (barcodeErrors > 0)
            {
                summary += "Tube scanning showed errors";
            }

            if (weighingErrors > 0)
            {
                if (summary.Length > 0)
                {
                    summary += Environment.NewLine;
                }

                summary += "Weighing showed errors";
            }

            ErrorSummary = summary;
        }
    }

    public void UpdateButtons() { }
}
