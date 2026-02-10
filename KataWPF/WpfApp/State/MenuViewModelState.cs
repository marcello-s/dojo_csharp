#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public class MenuViewModelState
{
    public CommandState? ExportState { get; set; }
    public CommandState? ImportState { get; set; }
    public string? NotificationText { get; set; }

    public void SetWelcomeState()
    {
        ExportState = new CommandState(false, false, null);
        ImportState = new CommandState(false, false, null);
        NotificationText = string.Empty;
    }

    public void EnableImport()
    {
        ImportState = new CommandState(true, true, null);
    }

    public void SetNotification(string text)
    {
        NotificationText = text;
    }
}
