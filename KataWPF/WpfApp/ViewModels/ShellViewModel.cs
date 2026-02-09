#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Windows;
using ViewModelLib;
using ViewModelLib.Navigation;

namespace WpfApp.ViewModels;

[Export(typeof(IShell))]
public class ShellViewModel : ScreenConductor, IShell
{
    private object navigationVm;
    private object menuVm;

    [ImportingConstructor]
    public ShellViewModel(
        NavigationViewModel navigationVm,
        MenuViewModel menuVm,
        WelcomeViewModel welcomeVm
    )
    {
        this.navigationVm = navigationVm;
        this.menuVm = menuVm;

        SetupNavigation();
        OpenScreen(welcomeVm);
    }

    public string Title => "Kata WPF Application";

    public bool IsBusy { get; set; }

    public bool CanClose()
    {
        return ActiveScreen.CanClose();
    }

    public object NavigationVm
    {
        get { return navigationVm; }
        set
        {
            navigationVm = value;
            NotifyOfPropertyChange(() => NavigationVm);
        }
    }

    public object MenuVm
    {
        get { return menuVm; }
        set
        {
            menuVm = value;
            NotifyOfPropertyChange(() => MenuVm);
        }
    }

    public void SetupNavigation()
    {
        try
        {
            var navigator = IoC.GetInstance<IScreenNavigator>();
            if (navigator == null)
            {
                System.Diagnostics.Trace.WriteLine("navigator is null");
                return;
            }

            var itemWelcome = new NavigationItem(typeof(WelcomeViewModel));
            var itemRange = new NavigationItem(typeof(SelectRangeViewModel));
            var itemOverview = new NavigationItem(typeof(ProcessingOverviewViewModel));
            var itemTaraDetails = new NavigationItem(typeof(TareProcessingDetailsViewModel));
            var itemWeighSolidDetails = new NavigationItem(
                typeof(WeighSolidProcessingDetailsViewModel)
            );
            var itemDiluteDetails = new NavigationItem(typeof(DiluteProcessingDetailsViewModel));
            itemRange.Previous = itemWelcome;
            itemRange.Next = itemOverview;
            itemOverview.Previous = itemRange;
            itemOverview.Next = itemWelcome;
            itemTaraDetails.Previous = itemOverview;
            itemTaraDetails.Next = itemWelcome;
            itemWeighSolidDetails.Previous = itemOverview;
            itemWeighSolidDetails.Next = itemWelcome;
            itemDiluteDetails.Previous = itemOverview;
            itemDiluteDetails.Next = itemWelcome;
            navigator.Add(itemWelcome);
            navigator.Add(itemRange);
            navigator.Add(itemOverview);
            navigator.Add(itemTaraDetails);
            navigator.Add(itemWeighSolidDetails);
            navigator.Add(itemDiluteDetails);

            navigator.NavigateTo(itemWelcome.Target!);
            if (navigator.Current != null)
            {
                System.Diagnostics.Trace.WriteLine(
                    "navigator current: " + navigator.Current.Target!.ToString()
                );
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.Message);
        }
    }
}
