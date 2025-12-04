#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Collections.Concurrent;

namespace KataEventAggregator;

/// <summary>
/// EventAggregator publish/subscribe pattern.
/// </summary>
internal class EventAggregator : MarshalByRefObject, IEventAggregator
{
    private readonly ConcurrentDictionary<Type, object> subjects =
        new ConcurrentDictionary<Type, object>();

    /// <summary>
    /// Return an event, IObservable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">The type of IObservable</typeparam>
    /// <returns>IObservable</returns>
    public IObservable<T> GetEvent<T>()
    {
        var subject = (ISubject<T>)subjects.GetOrAdd(typeof(T), t => new Subject<T>());
        return (IObservable<T>)subject;
    }

    /// <summary>
    /// Publish a message of type T
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    public void Publish<T>(T message)
    {
        object? subject;
        if (subjects.TryGetValue(typeof(T), out subject))
        {
            if (subject == null)
            {
                return;
            }

            ((ISubject<T>)subject).OnNext(message);
        }
    }
}
