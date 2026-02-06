#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Windows;

namespace ViewModelLib;

public static class Execute
{
    private static Action<Action> executor = action => action();

    public static void InitializeWithDispatcher()
    {
        var dispatcher = Application.Current.Dispatcher;
        executor = action =>
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        };
    }

    public static void OnUIThread(this Action action)
    {
        executor(action);
    }
}
