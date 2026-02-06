#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class GenericMessage<T> : MessageBase
{
    public GenericMessage(T content)
        : base()
    {
        Content = content;
    }

    public GenericMessage(object sender, object target, T content)
        : base(sender, target)
    {
        Content = content;
    }

    public T Content { get; private set; }
}
