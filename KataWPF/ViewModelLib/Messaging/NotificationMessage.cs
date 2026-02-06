#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class NotificationMessage : MessageBase
{
    public NotificationMessage(string notification)
        : base()
    {
        Notification = notification;
    }

    public NotificationMessage(object sender, object target, string notification)
        : base(sender, target)
    {
        Notification = notification;
    }

    public string Notification { get; private set; }
}
