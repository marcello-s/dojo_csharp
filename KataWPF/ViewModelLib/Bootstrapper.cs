#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Windows;

namespace ViewModelLib;

public static class Bootstrapper
{
    public static UIElement CreateShell()
    {
        IoC.InitializeWithMef();
        Execute.InitializeWithDispatcher();

        var shell = IoC.GetInstance<IShell>();
        if (shell == null)
        {
            throw new InvalidOperationException("No IShell implementation found");
        }

        var view = ViewLocator.Locate(shell);
        if (shell is ViewModelBase viewModel)
        {
            ViewModelBinder.CustomFindControl = viewModel.FindControl;
            ViewModelBinder.Bind(shell, view);
        }

        return view;
    }
}
