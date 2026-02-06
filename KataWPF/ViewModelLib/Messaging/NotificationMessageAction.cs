#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class NotificationMessageAction<T> : NotificationMessageWithCallback
{
    public NotificationMessageAction(string notification, Action<T> callback)
        : base(notification, callback) { }

    public NotificationMessageAction(
        object sender,
        object target,
        string notification,
        Action<T> callback
    )
        : base(sender, target, notification, callback) { }

    public object? Execute(T parameter)
    {
        if (parameter is not null)
        {
            return base.Execute(parameter);
        }

        return base.Execute();
    }
}
