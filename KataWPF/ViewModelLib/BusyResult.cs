#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib;

public class BusyResult(bool isBusy) : IResult
{
    public void Execute()
    {
        var shell = IoC.GetInstance<IShell>();
        shell?.IsBusy = isBusy;
        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
