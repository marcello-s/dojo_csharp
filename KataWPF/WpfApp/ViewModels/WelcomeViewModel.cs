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
using WpfApp.State;

namespace WpfApp.ViewModels;

[Export(typeof(WelcomeViewModel))]
public class WelcomeViewModel : ViewModelBase, IScreen
{
    public void Activate()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        var navigationState = new NavigationViewModelState();
        navigationState.SetWelcomeState();
        broker?.Send(new GenericMessage<NavigationViewModelState>(navigationState));

        var menuState = new MenuViewModelState();
        menuState.SetWelcomeState();
        broker?.Send(new GenericMessage<MenuViewModelState>(menuState));
    }

    public bool CanClose()
    {
        return true;
    }

    public void Deactivate() { }
}
