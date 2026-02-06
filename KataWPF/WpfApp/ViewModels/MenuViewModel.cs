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

[Export(typeof(MenuViewModel))]
public class MenuViewModel : ViewModelBase
{
    private CommandState exportState = new CommandState(true, false, false);
    private CommandState importState = new CommandState(true, false, false);
    private bool notification;
    private string notificationText = string.Empty;

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
        get { return exportState.CanExecute; }
        set
        {
            exportState = exportState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanExport);
        }
    }

    public bool ExportVisibility
    {
        get { return exportState.IsVisible; }
        set
        {
            exportState = exportState with { IsVisible = value };
            NotifyOfPropertyChange(() => ExportVisibility);
        }
    }

    public bool CanImport
    {
        get { return importState.CanExecute; }
        set
        {
            importState = importState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanImport);
        }
    }

    public bool ImportVisibility
    {
        get { return importState.IsVisible; }
        set
        {
            importState = importState with { IsVisible = value };
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
        get { return notificationText; }
        set
        {
            notificationText = value;
            NotifyOfPropertyChange(() => NotificationText);
        }
    }
}
