#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public class ScreenConductor : ViewModelBase
{
    private IScreen activeScreen = null!;

    public IScreen ActiveScreen
    {
        get { return activeScreen; }
        set { OpenScreen(value); }
    }

    public void OpenScreen(IScreen screen)
    {
        if (screen == null)
        {
            return;
        }

        if (screen.Equals(activeScreen))
        {
            return;
        }

        if (activeScreen != null && !activeScreen.CanClose())
        {
            return;
        }

        if (activeScreen != null)
        {
            activeScreen.Deactivate();
        }

        screen.Activate();
        activeScreen = screen;
        NotifyOfPropertyChange(() => ActiveScreen);
    }
}
