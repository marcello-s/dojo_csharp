#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using ViewModelLib.Messaging;

namespace ViewModelLib;

public class NotificationResult(string notification) : IResult
{
    public void Execute()
    {
        var broker = IoC.GetInstance<IMessageBroker>();
        broker?.Send(new NotificationMessage(notification));
        Completed(this, EventArgs.Empty);
    }

    public event EventHandler Completed = delegate { };
}
