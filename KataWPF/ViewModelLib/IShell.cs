#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public interface IShell
{
    bool IsBusy { get; set; }
    bool CanClose();
    void OpenScreen(IScreen screen);
}
