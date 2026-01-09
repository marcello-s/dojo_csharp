#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// EventAggregator publish/subscribe pattern.
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// Return an event, IObservable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">The type of IObservable</typeparam>
    /// <returns>IObservable</returns>
    IObservable<T> GetEvent<T>();

    /// <summary>
    /// Publish a message of type T
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    void Publish<T>(T message);
}
