#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public class ShowScreenResult(Type screen) : IResult
{
    private Action<IScreen> configuration = null!;

    public ShowScreenResult Configured(Action<IScreen> configuration)
    {
        this.configuration = configuration;
        return this;
    }

    public void Execute()
    {
        var shell = IoC.GetInstance<IShell>();
        var screenInstance = IoC.GetInstances(screen).First().Value as IScreen;

        if (screenInstance == null)
        {
            throw new InvalidOperationException(
                $"The requested screen '{screen.FullName}' is not registered in the IoC container."
            );
        }

        if (configuration != null)
        {
            configuration(screenInstance);
        }

        shell?.OpenScreen(screenInstance);
        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
