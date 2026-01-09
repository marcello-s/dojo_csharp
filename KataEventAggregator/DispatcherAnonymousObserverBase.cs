#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

// dotnet core does not have System.Windows.Threading by default
/*
using System.Windows.Threading;

namespace KataEventAggregator;

/// <summary>
/// An anonymous observer internally used when subscribed with callback action only.
/// The call for OnNext is pushed to the UI thread.
/// </summary>
/// <typeparam name="T">The data type of the observer</typeparam>
internal class DispatcherAnonymousObserverBase<T> : AnonymousObserverBase<T>
{
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Create instance
    /// </summary>
    /// <param name="onNextAction">The callback action for OnNext</param>
    public DispatcherAnonymousObserverBase(Action<T> onNextAction)
        : base(onNextAction)
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
    }

    /// <summary>
    /// Internal method to do OnNext.
    /// The call is pushed to the UI thread if needed.
    /// </summary>
    /// <param name="value">The current notification information</param>
    protected override void DoOnNext(T value)
    {
        if (_dispatcher.CheckAccess())
        {
            base.DoOnNext(value);
        }
        else
        {
            _dispatcher.Invoke(new Action(() => this.DoOnNext(value)));
        }
    }
}
*/
