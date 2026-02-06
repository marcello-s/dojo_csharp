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

[Export(typeof(NavigationViewModel))]
public class NavigationViewModel : ViewModelBase
{
    private CommandState backState = new CommandState(true, false, false);
    private CommandState selectTareState = new CommandState(true, true, false);
    private CommandState selectWeighSolidState = new CommandState(true, true, false);
    private CommandState selectDiluteState = new CommandState(true, true, false);
    private CommandState goState = new CommandState(true, false, false);

    public override DependencyObject? CustomFindControl(
        FrameworkElement view,
        System.Reflection.PropertyInfo property
    )
    {
        if (!ViewModelBinder.PropertyRegistry.ContainsKey(typeof(AppShell.Controls.TouchButton)))
        {
            ViewModelBinder.PropertyRegistry.Add(
                typeof(AppShell.Controls.TouchButton),
                new HashSet<DependencyProperty> { AppShell.Controls.TouchButton.VisibilityProperty }
            );
        }
        if (
            !ViewModelBinder.PropertyRegistry.ContainsKey(
                typeof(AppShell.Controls.ToggleTouchButton)
            )
        )
        {
            ViewModelBinder.PropertyRegistry.Add(
                typeof(AppShell.Controls.ToggleTouchButton),
                new HashSet<DependencyProperty>
                {
                    AppShell.Controls.ToggleTouchButton.VisibilityProperty,
                    AppShell.Controls.ToggleTouchButton.IsCheckedProperty,
                }
            );
        }

        if (property.Name.StartsWith("Back"))
        {
            return view.FindName("Back") as DependencyObject;
        }

        if (property.Name.StartsWith("Go"))
        {
            return view.FindName("Go") as DependencyObject;
        }

        if (property.Name.Contains("SelectTare") && !property.Name.StartsWith("Can"))
        {
            return view.FindName("SelectTare") as DependencyObject;
        }

        if (property.Name.Contains("SelectWeighSolid") && !property.Name.StartsWith("Can"))
        {
            return view.FindName("SelectWeighSolid") as DependencyObject;
        }

        if (property.Name.Contains("SelectDilute") && !property.Name.StartsWith("Can"))
        {
            return view.FindName("SelectDilute") as DependencyObject;
        }

        return base.CustomFindControl(view, property);
    }

    public bool CanBack
    {
        get { return backState.CanExecute; }
        set
        {
            backState = backState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanBack);
        }
    }

    public bool BackVisibility
    {
        get { return backState.IsVisible; }
        set
        {
            backState = backState with { IsVisible = value };
            NotifyOfPropertyChange(() => BackVisibility);
        }
    }

    public bool CanSelectTare
    {
        get { return selectTareState.CanExecute; }
        set
        {
            selectTareState = selectTareState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectTare);
        }
    }

    public bool SelectTareVisibility
    {
        get { return selectTareState.IsVisible; }
        set
        {
            selectTareState = selectTareState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectTareVisibility);
        }
    }

    public bool IsSelectTareChecked
    {
        get { return selectTareState.IsChecked ?? false; }
        set
        {
            selectTareState = selectTareState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsSelectTareChecked);
        }
    }

    public bool CanSelectWeighSolid
    {
        get { return selectWeighSolidState.CanExecute; }
        set
        {
            selectWeighSolidState = selectWeighSolidState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectWeighSolid);
        }
    }

    public bool SelectWeighSolidVisibility
    {
        get { return selectWeighSolidState.IsVisible; }
        set
        {
            selectWeighSolidState = selectWeighSolidState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectWeighSolidVisibility);
        }
    }

    public bool IsSelectWeighSolidChecked
    {
        get { return selectWeighSolidState.IsChecked ?? false; }
        set
        {
            selectWeighSolidState = selectWeighSolidState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsSelectWeighSolidChecked);
        }
    }

    public bool CanSelectDilute
    {
        get { return selectDiluteState.CanExecute; }
        set
        {
            selectDiluteState = selectDiluteState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectDilute);
        }
    }

    public bool SelectDiluteVisibility
    {
        get { return selectDiluteState.IsVisible; }
        set
        {
            selectDiluteState = selectDiluteState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectDiluteVisibility);
        }
    }

    public bool IsDiluteChecked
    {
        get { return selectDiluteState.IsChecked ?? false; }
        set
        {
            selectDiluteState = selectDiluteState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsDiluteChecked);
        }
    }

    public bool CanGo
    {
        get { return goState.CanExecute; }
        set
        {
            goState = goState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanGo);
        }
    }

    public bool GoVisibility
    {
        get { return goState.IsVisible; }
        set
        {
            goState = goState with { IsVisible = value };
            NotifyOfPropertyChange(() => GoVisibility);
        }
    }
}
