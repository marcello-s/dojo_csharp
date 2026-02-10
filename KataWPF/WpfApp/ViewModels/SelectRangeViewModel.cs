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
using WpfApp.ActionResults;
using WpfApp.State;

namespace WpfApp.ViewModels;

[Export(typeof(SelectRangeViewModel))]
[method: ImportingConstructor]
public class SelectRangeViewModel(SharedState state) : ViewModelBase, IScreen
{
    private string title = "Select Range ViewModel";
    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            NotifyOfPropertyChange(() => Title);
        }
    }

    private string gridRange = "10 - 11";
    public string GridRange
    {
        get { return gridRange; }
        set
        {
            gridRange = value;
            NotifyOfPropertyChange(() => GridRange);
        }
    }

    private bool allTubes;
    public bool AllTubes
    {
        get { return allTubes; }
        set
        {
            allTubes = value;
            NotifyOfPropertyChange(() => AllTubes);
            Validate();
        }
    }

    private bool specifyRange;
    public bool SpecifyRange
    {
        get { return specifyRange; }
        set
        {
            specifyRange = value;
            NotifyOfPropertyChange(() => SpecifyRange);
            NotifyOfPropertyChange(() => IsTubeRangeEnabled);
            Validate();
        }
    }

    public bool IsTubeRangeEnabled
    {
        get { return specifyRange; }
    }

    private double tubeRangeMinimum = 1;
    public double TubeRangeMinimum
    {
        get { return tubeRangeMinimum; }
        set
        {
            tubeRangeMinimum = value;
            NotifyOfPropertyChange(() => TubeRangeMinimum);
        }
    }

    private double tubeRangeMaximum = 160;
    public double TubeRangeMaximum
    {
        get { return tubeRangeMaximum; }
        set
        {
            tubeRangeMaximum = value;
            NotifyOfPropertyChange(() => TubeRangeMaximum);
        }
    }

    private double tubeRangeLowerValue = 1;
    public double TubeRangeLowerValue
    {
        get { return tubeRangeLowerValue; }
        set
        {
            tubeRangeLowerValue = value;
            NotifyOfPropertyChange(() => TubeRangeLowerValue);
            NotifyOfPropertyChange(() => RangeLowerValueText);
        }
    }

    private double tubeRangeUpperValue = 160;
    public double TubeRangeUpperValue
    {
        get { return tubeRangeUpperValue; }
        set
        {
            tubeRangeUpperValue = value;
            NotifyOfPropertyChange(() => TubeRangeUpperValue);
            NotifyOfPropertyChange(() => RangeUpperValueText);
        }
    }

    public string RangeLowerValueText
    {
        get { return "[" + (int)TubeRangeLowerValue + "]"; }
    }

    public string RangeUpperValueText
    {
        get { return "[" + (int)TubeRangeUpperValue + "]"; }
    }

    public override DependencyObject? CustomFindControl(
        FrameworkElement view,
        System.Reflection.PropertyInfo property
    )
    {
        if (!ViewModelBinder.PropertyRegistry.ContainsKey(typeof(AppShell.Controls.RangeSlider)))
        {
            ViewModelBinder.PropertyRegistry.Add(
                typeof(AppShell.Controls.RangeSlider),
                new HashSet<DependencyProperty>
                {
                    AppShell.Controls.RangeSlider.LowerValueProperty,
                    AppShell.Controls.RangeSlider.UpperValueProperty,
                    AppShell.Controls.RangeSlider.MinimumProperty,
                    AppShell.Controls.RangeSlider.MaximumProperty,
                    AppShell.Controls.RangeSlider.IsEnabledProperty,
                }
            );
        }

        if (property.Name.Contains("TubeRange"))
        {
            return view.FindName("TubeRange") as DependencyObject;
        }

        return base.CustomFindControl(view, property);
    }

    public void Activate()
    {
        AllTubes = state.AllTubesSelected;
        TubeRangeLowerValue = state.RangeTubesLowerValue;
        TubeRangeUpperValue = state.RangeTubesUpperValue;

        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Send(
            new GenericMessage<NavigationViewModelState>(GetNavigationStateForMode(state.Mode))
        );
        broker?.Register<NotificationMessageAction<IEnumerable<IResult>>>(
            this,
            HandleNotificationActions
        );
        broker?.Register<NotificationMessageAction<IResult>>(this, HandleNotificationAction);

        // enable import menu
        if (state.Mode == ModeEnum.WeighSolid || state.Mode == ModeEnum.Dilute)
        {
            var menuState = new MenuViewModelState();
            menuState.EnableImport();
            broker?.Send(new GenericMessage<MenuViewModelState>(menuState));
        }

        Validate();
    }

    public void Deactivate()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
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

    public void Validate()
    {
        if (allTubes || specifyRange)
        {
            var broker = IoC.GetInstance<IMessageBroker>();
            var navigationState = new NavigationViewModelState();
            navigationState.SetGoState();
            broker?.Send(new GenericMessage<NavigationViewModelState>(navigationState));
        }
    }

    public void WithMode(ModeEnum mode)
    {
        state.Mode = mode;
        Title = mode.ToString();
    }

    public static NavigationViewModelState GetNavigationStateForMode(ModeEnum mode)
    {
        var navigationState = new NavigationViewModelState();
        switch (mode)
        {
            case ModeEnum.Tare:
                navigationState.SetSelectTareState();
                break;
            case ModeEnum.WeighSolid:
                navigationState.SetSelectWeighSolidState();
                break;
            case ModeEnum.Dilute:
                navigationState.SetSelectDiluteState();
                break;
            default:
                navigationState.SetWelcomeState();
                break;
        }

        return navigationState;
    }

    public void HandleNotificationActions(NotificationMessageAction<IEnumerable<IResult>> message)
    {
        if (message.Notification.Equals(NavigationEventEnum.BeginGo.ToString()))
        {
            var results = new List<IResult>()
            {
                IoC.GetInstance<SelectRangeResult>()!
                    .With(
                        AllTubes,
                        TubeRangeLowerValue,
                        TubeRangeUpperValue,
                        TubeRangeMinimum,
                        TubeRangeMaximum
                    ),
            };

            message.Execute(results);
        }
    }

    public static void HandleNotificationAction(NotificationMessageAction<IResult> message)
    {
        if (message.Notification.Equals(MenuEventEnum.Import.ToString()))
        {
            message.Execute(IoC.GetInstance<ImportResult>()!);
        }
    }
}
