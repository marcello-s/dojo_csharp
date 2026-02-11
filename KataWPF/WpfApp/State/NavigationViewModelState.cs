#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public class NavigationViewModelState
{
    public CommandState? SelectTareState { get; set; }
    public CommandState? SelectWeighSolidState { get; set; }
    public CommandState? SelectDiluteState { get; set; }

    public CommandState? GoState { get; set; }
    public CommandState? BackState { get; set; }

    public void SetWelcomeState()
    {
        SelectTareState = new CommandState(true, true, false);
        SelectWeighSolidState = new CommandState(true, true, false);
        SelectDiluteState = new CommandState(true, true, false);
        GoState = new CommandState(true, false, null);
        BackState = new CommandState(true, false, null);
    }

    public void SetSelectTareState()
    {
        SelectTareState = new CommandState(false, true, true);
        SelectWeighSolidState = new CommandState(false, false, false);
        SelectDiluteState = new CommandState(false, false, false);
        GoState = new CommandState(false, true, null);
        BackState = new CommandState(true, true, null);
    }

    public void SetSelectWeighSolidState()
    {
        SelectTareState = new CommandState(false, false, false);
        SelectWeighSolidState = new CommandState(false, true, true);
        SelectDiluteState = new CommandState(false, false, false);
        GoState = new CommandState(true, true, null);
        BackState = new CommandState(true, true, null);
    }

    public void SetSelectDiluteState()
    {
        SelectTareState = new CommandState(false, false, false);
        SelectWeighSolidState = new CommandState(false, false, false);
        SelectDiluteState = new CommandState(false, true, true);
        GoState = new CommandState(false, true, null);
        BackState = new CommandState(true, true, null);
    }

    public void EnableGo()
    {
        GoState = new CommandState(true, true, false);
    }

    public void DisableGo()
    {
        GoState = new CommandState(false, true, false);
    }

    public void EnableBack()
    {
        BackState = new CommandState(true, true, null);
    }

    public void DisableBack()
    {
        BackState = new CommandState(false, true, null);
    }
}
