#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public interface IMessageBroker
{
    void Register<T>(object recipient, Action<T> action);
    void Unregister(object recipient);
    void Unregister<T>(object recipient);
    void Unregister<T>(object recipient, Action<T> action);
    void Send<T>(T message);
    void Send<TMessage, TTarget>(TMessage message);
}
