#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// An anonymous observer internally used when subscribed with callback action only.
/// </summary>
/// <typeparam name="T">The data type of the observer</typeparam>
internal class AnonymousObserverBase<T> : MarshalByRefObject, IObserver<T>
{
    private readonly Action<T> _onNextAction;

    /// <summary>
    /// Create instance
    /// </summary>
    /// <param name="onNextAction">The callback action for OnNext</param>
    public AnonymousObserverBase(Action<T> onNextAction)
    {
        _onNextAction = onNextAction;
    }

    /// <summary>
    /// Provides the observer with new data.
    /// </summary>
    /// <param name="value">The current notification information</param>
    public void OnNext(T value)
    {
        DoOnNext(value);
    }

    /// <summary>
    /// Internal method to do OnNext.
    /// </summary>
    /// <param name="value">The current notification information</param>
    protected virtual void DoOnNext(T value)
    {
        _onNextAction(value);
    }

    /// <summary>
    /// Notifies the observer that the provider has experienced an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error</param>
    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Notifies the observer that the provider has finished sending push-based notifications.
    /// </summary>
    public void OnCompleted()
    {
        throw new NotImplementedException();
    }
}
