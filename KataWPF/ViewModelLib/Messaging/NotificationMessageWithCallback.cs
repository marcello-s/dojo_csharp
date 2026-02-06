#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class NotificationMessageWithCallback : NotificationMessage, ICallbackMessage
{
    private readonly Delegate callback;

    public NotificationMessageWithCallback(string notification, Delegate callback)
        : base(notification)
    {
        CheckCallback(callback);
        this.callback = callback;
    }

    public NotificationMessageWithCallback(
        object sender,
        object target,
        string notification,
        Delegate callback
    )
        : base(sender, target, notification)
    {
        CheckCallback(callback);
        this.callback = callback;
    }

    internal static void CheckCallback(Delegate callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException("callback", "callback must not be null");
        }
    }

    public virtual object? Execute(params object[] arguments)
    {
        return callback.DynamicInvoke(arguments);
    }
}
