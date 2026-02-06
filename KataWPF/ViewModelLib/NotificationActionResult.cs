#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using ViewModelLib.Messaging;

namespace ViewModelLib;

public class NotificationActionResult(string notification, Action<IResult> callback) : IResult
{
    public void Execute()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Send(new NotificationMessageAction<IResult>(notification, callback));
        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
