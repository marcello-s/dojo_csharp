#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public struct NavigationViewModelState
{
    public CommandState SelectTareState { get; set; }
    public CommandState SelectWeighSolidState { get; set; }
    public CommandState SelectDiluteState { get; set; }

    public CommandState GoState { get; set; }
    public CommandState BackState { get; set; }

    public void SetWelcomeState()
    {
        SelectTareState = new CommandState(true, true, false);
        SelectWeighSolidState = new CommandState(true, true, false);
        SelectDiluteState = new CommandState(true, true, false);
        GoState = new CommandState(true, false, null);
        BackState = new CommandState(true, false, null);
    }
}
