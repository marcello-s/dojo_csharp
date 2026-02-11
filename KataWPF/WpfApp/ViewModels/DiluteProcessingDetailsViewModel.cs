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
using ViewModelLib.Services;
using WpfApp.ActionResults;
using WpfApp.State;

namespace WpfApp.ViewModels;

[Export(typeof(DiluteProcessingDetailsViewModel))]
[method: ImportingConstructor]
public class DiluteProcessingDetailsViewModel(SharedState state, ILocalizationService localization)
    : ViewModelBase,
        IScreen
{
    private string title = "Dilute Processing Details ViewModel";
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

    public GridRecordCollection GetDiluteDetails
    {
        get { return recordCollection; }
    }

    public void Activate()
    {
        Title = localization.TranslateString(() => Title, state.Mode.ToString());
        if (state.ProcessingDataList != null)
        {
            GridHelper.SetupGridData(state.ProcessingDataList, out recordCollection);
        }

        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Register<NotificationMessage>(this, HandleNotification);
        broker?.Register<NotificationMessageAction<IEnumerable<IResult>>>(
            this,
            HandleNotificationActions
        );
        broker?.Register<NotificationMessageAction<IResult>>(this, HandleNotificationAction);

        var enableBack = new NavigationViewModelState();
        enableBack.EnableBack();
        broker?.Send(new GenericMessage<NavigationViewModelState>(enableBack));
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

    public void HandleNotification(NotificationMessage message)
    {
        if (message.Notification.Equals(SharedState.UpdatedMessage))
        {
            if (state.ProcessingDataList != null)
            {
                GridHelper.SetupGridData(state.ProcessingDataList, out recordCollection);
                NotifyOfPropertyChange(() => GetDiluteDetails);
            }

            UpdateButtons();
        }
    }

    public void HandleNotificationActions(NotificationMessageAction<IEnumerable<IResult>> message)
    {
        if (message.Notification.Equals(NavigationEventEnum.BeginGo.ToString()))
        {
            var results = new List<IResult>() { IoC.GetInstance<CreateWorklistResult>()! };

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

    public void UpdateButtons()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        if (state.ProcessingDataList != null && state.ProcessingDataList.Count > 0)
        {
            var enableImport = new MenuViewModelState();
            enableImport.EnableImport();
            broker?.Send(new GenericMessage<MenuViewModelState>(enableImport));

            var enableExport = new MenuViewModelState();
            enableExport.EnableExport();
            broker?.Send(new GenericMessage<MenuViewModelState>(enableExport));
        }
    }
}
