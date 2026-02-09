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
using WpfApp.State;

namespace WpfApp.ViewModels;

[Export(typeof(NavigationViewModel))]
public class NavigationViewModel : ViewModelBase
{
    private NavigationViewModelState state = new NavigationViewModelState();
    private IList<IResult> results = new List<IResult>();

    public NavigationViewModel()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Register<GenericMessage<NavigationViewModelState>>(this, ApplyState);
    }

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
        get { return state.BackState.CanExecute; }
        set
        {
            state.BackState = state.BackState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanBack);
        }
    }

    public bool BackVisibility
    {
        get { return state.BackState.IsVisible; }
        set
        {
            state.BackState = state.BackState with { IsVisible = value };
            NotifyOfPropertyChange(() => BackVisibility);
        }
    }

    public bool CanSelectTare
    {
        get { return state.SelectTareState.CanExecute; }
        set
        {
            state.SelectTareState = state.SelectTareState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectTare);
        }
    }

    public bool SelectTareVisibility
    {
        get { return state.SelectTareState.IsVisible; }
        set
        {
            state.SelectTareState = state.SelectTareState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectTareVisibility);
        }
    }

    public bool IsSelectTareChecked
    {
        get { return state.SelectTareState.IsChecked ?? false; }
        set
        {
            state.SelectTareState = state.SelectTareState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsSelectTareChecked);
        }
    }

    public IEnumerable<IResult> SelectTare()
    {
        var navigator = IoC.GetInstance<IScreenNavigator>();
        navigator?.NavigateTo(typeof(SelectRangeViewModel));

        yield return new ShowScreenResultGeneric<SelectRangeViewModel>().Configured(vm =>
            vm.WithMode(ModeEnum.Tare)
        );
    }

    public bool CanSelectWeighSolid
    {
        get { return state.SelectWeighSolidState.CanExecute; }
        set
        {
            state.SelectWeighSolidState = state.SelectWeighSolidState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectWeighSolid);
        }
    }

    public bool SelectWeighSolidVisibility
    {
        get { return state.SelectWeighSolidState.IsVisible; }
        set
        {
            state.SelectWeighSolidState = state.SelectWeighSolidState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectWeighSolidVisibility);
        }
    }

    public bool IsSelectWeighSolidChecked
    {
        get { return state.SelectWeighSolidState.IsChecked ?? false; }
        set
        {
            state.SelectWeighSolidState = state.SelectWeighSolidState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsSelectWeighSolidChecked);
        }
    }

    public bool CanSelectDilute
    {
        get { return state.SelectDiluteState.CanExecute; }
        set
        {
            state.SelectDiluteState = state.SelectDiluteState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanSelectDilute);
        }
    }

    public bool SelectDiluteVisibility
    {
        get { return state.SelectDiluteState.IsVisible; }
        set
        {
            state.SelectDiluteState = state.SelectDiluteState with { IsVisible = value };
            NotifyOfPropertyChange(() => SelectDiluteVisibility);
        }
    }

    public bool IsDiluteChecked
    {
        get { return state.SelectDiluteState.IsChecked ?? false; }
        set
        {
            state.SelectDiluteState = state.SelectDiluteState with { IsChecked = value };
            NotifyOfPropertyChange(() => IsDiluteChecked);
        }
    }

    public bool CanGo
    {
        get { return state.GoState.CanExecute; }
        set
        {
            state.GoState = state.GoState with { CanExecute = value };
            NotifyOfPropertyChange(() => CanGo);
        }
    }

    public bool GoVisibility
    {
        get { return state.GoState.IsVisible; }
        set
        {
            state.GoState = state.GoState with { IsVisible = value };
            NotifyOfPropertyChange(() => GoVisibility);
        }
    }

    public void ApplyState(GenericMessage<NavigationViewModelState> message)
    {
        state = message.Content;
        CanSelectTare = state.SelectTareState.CanExecute;
        SelectTareVisibility = state.SelectTareState.IsVisible;
        IsSelectTareChecked = state.SelectTareState.IsChecked ?? false;

        CanSelectWeighSolid = state.SelectWeighSolidState.CanExecute;
        SelectWeighSolidVisibility = state.SelectWeighSolidState.IsVisible;
        IsSelectWeighSolidChecked = state.SelectWeighSolidState.IsChecked ?? false;

        CanSelectDilute = state.SelectDiluteState.CanExecute;
        SelectDiluteVisibility = state.SelectDiluteState.IsVisible;
        IsDiluteChecked = state.SelectDiluteState.IsChecked ?? false;

        CanBack = state.BackState.CanExecute;
        BackVisibility = state.BackState.IsVisible;
        CanGo = state.GoState.CanExecute;
        GoVisibility = state.GoState.IsVisible;
    }

    public void EnqueuResult(IEnumerable<IResult> results)
    {
        if (results == null)
        {
            return;
        }

        foreach (var r in results)
        {
            this.results.Add(r);
        }
    }
}
