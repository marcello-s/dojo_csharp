#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// An anonymous observer internally used when subscribed with callback action only.
/// The call for OnNext is pushed to a ThreadPool thread.
/// </summary>
/// <typeparam name="T">The data type of the observer</typeparam>
internal class BackgroundAnonymousObserverBase<T> : AnonymousObserverBase<T>
{
    /// <summary>
    /// Create instance
    /// </summary>
    /// <param name="onNextAction">The callback action for OnNext</param>
    public BackgroundAnonymousObserverBase(Action<T> onNextAction)
        : base(onNextAction) { }

    /// <summary>
    /// Internal method to do OnNext.
    /// Invokes the specified OnNext( value ) in an asynchronous thread by using a <see cref="ThreadPool"/>.
    /// </summary>
    /// <param name="value">The current notification information</param>
    protected override void DoOnNext(T value)
    {
        ThreadPool.QueueUserWorkItem(o => this.OnNext(value));
    }
}
