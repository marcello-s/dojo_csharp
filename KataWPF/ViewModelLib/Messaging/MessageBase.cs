#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class MessageBase
{
    public MessageBase() { }

    public MessageBase(object sender, object target)
    {
        Sender = sender;
        Target = target;
    }

    public object? Sender { get; private set; }
    public object? Target { get; private set; }
}
