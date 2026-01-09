#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public interface IEventAggregator
{
    Action<Action> PublicationThreadMarshaler { get; set; }
    void Subscribe(object? instance);
    void Unsubscribe(object? instance);
    void Publish(object message);
    void Publish(object message, Action<Action> marshal);
}
