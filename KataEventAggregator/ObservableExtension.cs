#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// Extension methods to the IObservable interface.
/// </summary>
public static class ObservableExtension
{
    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// A callback action can be registered and runs on the specified thread option.
    /// </summary>
    /// <param name="observable">IObservable instance</param>
    /// <param name="onNextAction">Callback action for OnNext notification</param>
    /// <param name="threadOption">Specifies on which thread to run the callback method, default: publisher thread</param>
    /// <typeparam name="T">The data type of IObservable</typeparam>
    /// <returns>An implementation of IDisposable</returns>
    /// <exception cref="ArgumentNullException">Thrown if onNextAction is null</exception>
    public static IDisposable Subscribe<T>(
        this IObservable<T> observable,
        Action<T> onNextAction,
        ThreadOption threadOption = ThreadOption.PublisherThread
    )
    {
        if (onNextAction == null)
        {
            throw new ArgumentNullException("onNextAction");
        }

        var observer = new AnonymousObserverBase<T>(onNextAction);
        switch (threadOption)
        {
            /*
            case ThreadOption.UiThread:
                observer = new DispatcherAnonymousObserverBase<T>(onNextAction);
                break;
            */
            case ThreadOption.BackgroundThread:
                observer = new BackgroundAnonymousObserverBase<T>(onNextAction);
                break;
        }

        return observable.Subscribe(observer);
    }
}
