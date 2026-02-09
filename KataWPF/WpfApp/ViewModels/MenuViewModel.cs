#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using ViewModelLib;
using ViewModelLib.Messaging;
using WpfApp.State;

namespace WpfApp.ViewModels;

[Export(typeof(MenuViewModel))]
public class MenuViewModel : ViewModelBase
{
    private MenuViewModelState state = new MenuViewModelState();
    private bool notification;
    private IList<IResult> results = new List<IResult>();

    public MenuViewModel()
    {
        state.SetWelcomeState();
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Register<GenericMessage<MenuViewModelState>>(this, ApplyState);
    }

    public override DependencyObject? CustomFindControl(
        FrameworkElement view,
        System.Reflection.PropertyInfo property
    )
    {
        if (property.Name.StartsWith("Export"))
        {
            return view.FindName("Export") as DependencyObject;
        }

        if (property.Name.StartsWith("Import"))
        {
            return view.FindName("Import") as DependencyObject;
        }

        return base.CustomFindControl(view, property);
    }

    public bool CanExport
    {
        get { return state.ExportState.CanExecute; }
        set
        {
            state.ExportState = state.ExportState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanExport);
        }
    }

    public bool ExportVisibility
    {
        get { return state.ExportState.IsVisible; }
        set
        {
            state.ExportState = state.ExportState with { IsVisible = value };
            NotifyOfPropertyChange(() => ExportVisibility);
        }
    }

    public bool CanImport
    {
        get { return state.ImportState.CanExecute; }
        set
        {
            state.ImportState = state.ImportState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanImport);
        }
    }

    public bool ImportVisibility
    {
        get { return state.ImportState.IsVisible; }
        set
        {
            state.ImportState = state.ImportState with { IsVisible = value };
            NotifyOfPropertyChange(() => ImportVisibility);
        }
    }

    public bool Notification
    {
        get { return notification; }
        set
        {
            notification = value;
            NotifyOfPropertyChange(() => Notification);
        }
    }

    public string NotificationText
    {
        get { return state.NotificationText; }
        set
        {
            state.NotificationText = value;
            NotifyOfPropertyChange(() => NotificationText);
        }
    }

    public void ApplyState(GenericMessage<MenuViewModelState> message)
    {
        state = message.Content;
        CanExport = state.ExportState.CanExecute;
        ExportVisibility = state.ExportState.IsVisible;
        CanImport = state.ImportState.CanExecute;
        ImportVisibility = state.ImportState.IsVisible;

        NotificationText = state.NotificationText;
        Notification = !string.IsNullOrEmpty(state.NotificationText);
    }

    public void EnqueueResult(IResult result)
    {
        if (result != null)
        {
            results.Add(result);
        }
    }
}
