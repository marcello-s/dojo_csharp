#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;
using ViewModelLib;
using ViewModelLib.Messaging;
using ViewModelLib.Navigation;
using WpfApp.ActionResults;
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
        var navigator = IoC.GetInstance<IScreenNavigator>();
        switch (state.Mode)
        {
            case ModeEnum.Tare:
                navigator?.NavigateTo(typeof(TareProcessingDetailsViewModel));
                return new ShowScreenResultGeneric<TareProcessingDetailsViewModel>();
            case ModeEnum.WeighSolid:
                navigator?.NavigateTo(typeof(WeighSolidProcessingDetailsViewModel));
                return new ShowScreenResultGeneric<WeighSolidProcessingDetailsViewModel>();
            case ModeEnum.Dilute:
                navigator?.NavigateTo(typeof(DiluteProcessingDetailsViewModel));
                return new ShowScreenResultGeneric<DiluteProcessingDetailsViewModel>();
        }

        return null!;
    }

    public void Activate()
    {
        UpdateFigures();
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Register<NotificationMessage>(this, HandleNotification);
        broker?.Register<NotificationMessageAction<IEnumerable<IResult>>>(
            this,
            HandleNotificationActions
        );
        broker?.Register<NotificationMessageAction<IResult>>(this, HandleNotificationAction);
        var disableBack = new NavigationViewModelState();
        disableBack.DisableBack();
        broker?.Send(new GenericMessage<NavigationViewModelState>(disableBack));
        var showExport = new MenuViewModelState();
        showExport.ShowExport();
        broker?.Send(new GenericMessage<MenuViewModelState>(showExport));

        if (state.Mode == ModeEnum.Tare || state.Mode == ModeEnum.WeighSolid)
        {
            var hideImport = new MenuViewModelState();
            hideImport.HideImport();
            broker?.Send(new GenericMessage<MenuViewModelState>(hideImport));
        }

        if (state.Mode == ModeEnum.Dilute)
        {
            if (state.ProcessingDataList == null || state.ProcessingDataList.Count == 0)
            {
                ErrorSummary = "List does not contain any samples";
            }

            var disableGo = new NavigationViewModelState();
            disableGo.DisableGo();
            broker?.Send(new GenericMessage<NavigationViewModelState>(disableGo));

            var showImport = new MenuViewModelState();
            showImport.ShowImport();
            broker?.Send(new GenericMessage<MenuViewModelState>(showImport));
        }

        UpdateButtons();
    }

    public void Deactivate()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Unregister<NotificationMessage>(this, HandleNotification);
        broker?.Unregister<NotificationMessageAction<IEnumerable<IResult>>>(
            this,
            HandleNotificationActions
        );
        broker?.Unregister<NotificationMessageAction<IResult>>(this, HandleNotificationAction);
    }

    public bool CanClose()
    {
        return true;
    }

    private void HandleNotification(NotificationMessage message)
    {
        if (message.Notification.Equals(SharedState.UpdatedMessage))
        {
            UpdateFigures();
            UpdateButtons();
        }
    }

    public void HandleNotificationActions(NotificationMessageAction<IEnumerable<IResult>> message)
    {
        if (message.Notification.Equals(NavigationEventEnum.BeginGo.ToString()))
        {
            var results = new List<IResult>()
            {
                IoC.GetInstance<CreateWorklistResult>()!,
                IoC.GetInstance<CommitValidProcessingDataResult>()!,
            };
            message.Execute(results);
        }
    }

    public void HandleNotificationAction(NotificationMessageAction<IResult> message)
    {
        if (message.Notification.Equals(MenuEventEnum.Export.ToString()))
        {
            message.Execute(IoC.GetInstance<ExportResult>()!);
        }

        if (message.Notification.Equals(MenuEventEnum.Import.ToString()))
        {
            message.Execute(IoC.GetInstance<ImportConcentrationResult>()!);
        }
    }

    public void UpdateFigures()
    {
        if (state == null)
        {
            return;
        }

        barcodeErrors = NumberOfBarcodeErrors(state.ProcessingDataList);
        weighingErrors = NumberOfTaraErrors(state.ProcessingDataList);
        if (state.Mode == ModeEnum.WeighSolid)
        {
            weighingErrors += NumberOfWeightErrors(state.ProcessingDataList);
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

    public void UpdateButtons()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        if (
            state.ProcessingDataList != null
            && ContainsAnyValidProcessingData(state.ProcessingDataList)
        )
        {
            if (state.Mode == ModeEnum.Tare || state.Mode == ModeEnum.WeighSolid)
            {
                var enableExport = new MenuViewModelState();
                enableExport.EnableExport();
                broker?.Send(new GenericMessage<MenuViewModelState>(enableExport));
            }
        }
        if (
            state.ProcessingDataList != null
            && state.ProcessingDataList.Count > 0
            && state.Mode == ModeEnum.Dilute
        )
        {
            var enableGo = new NavigationViewModelState();
            enableGo.EnableGo();
            broker?.Send(new GenericMessage<NavigationViewModelState>(enableGo));

            var enableImport = new MenuViewModelState();
            enableImport.EnableImport();
            broker?.Send(new GenericMessage<MenuViewModelState>(enableImport));

            var enableExport = new MenuViewModelState();
            enableExport.EnableExport();
            broker?.Send(new GenericMessage<MenuViewModelState>(enableExport));
        }
    }

    public static int NumberOfBarcodeErrors(IEnumerable<ProcessingData> data)
    {
        return data.Sum(
            (p) => p.Barcode.Equals("***") || p.Status.ToLower().Contains("duplicate") ? 1 : 0
        );
    }

    public static int NumberOfTaraErrors(IEnumerable<ProcessingData> data)
    {
        return data.Sum((p) => p.Tara <= 0 ? 1 : 0);
    }

    public static int NumberOfWeightErrors(IEnumerable<ProcessingData> data)
    {
        return data.Sum((p) => p.SolidWeight <= p.Tara ? 1 : 0);
    }

    public static bool ContainsAnyValidProcessingData(IEnumerable<ProcessingData> data)
    {
        return data.Any((p) => p.Processing.Equals(ProcessingDataValidation.PROCESSING_PROCESS));
    }
}
