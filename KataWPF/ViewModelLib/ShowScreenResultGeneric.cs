#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public class ShowScreenResultGeneric<T> : IResult
    where T : IScreen
{
    private Action<T> configuration = null!;

    public ShowScreenResultGeneric<T> Configured(Action<T> configuration)
    {
        this.configuration = configuration;

        return this;
    }

    public void Execute()
    {
        var shell = IoC.GetInstance<IShell>();
        var screen = IoC.GetInstance<T>();

        if (screen != null)
        {
            if (configuration != null)
            {
                configuration(screen);
            }

            shell?.OpenScreen(screen);
        }

        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
