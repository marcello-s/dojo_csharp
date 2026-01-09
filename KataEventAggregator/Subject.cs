#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// Combine observer and observable
/// </summary>
/// <typeparam name="T">The data type of ISubject and IObservable</typeparam>
internal class Subject<T> : MarshalByRefObject, ISubject<T>, IObservable<T>
{
    private readonly List<ObserverUnsubscriberStruct> observerUnsubscriberList;

    /// <summary>
    /// Create instance
    /// </summary>
    internal Subject()
    {
        observerUnsubscriberList = new List<ObserverUnsubscriberStruct>();
    }

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications</param>
    /// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
    public IDisposable Subscribe(IObserver<T>? observer)
    {
        if (observer == null)
        {
            throw new ArgumentNullException("observer");
        }

        var observerWeakReference = new WeakReference(observer);
        var unsubscriber = new Unsubscriber(observerUnsubscriberList, observerWeakReference);
        var unsubscriberWeakReference = new WeakReference(unsubscriber);

        var observerUnsubscriberStruct = new ObserverUnsubscriberStruct
        {
            ObserverReference = observer as AnonymousObserverBase<T>,
            ObserverWeakReference = observerWeakReference,
            UnsubscriberWeakReference = unsubscriberWeakReference,
        };
        observerUnsubscriberList.Add(observerUnsubscriberStruct);

        return unsubscriber;
    }

    /// <summary>
    /// Provides the observer with new data.
    /// </summary>
    /// <param name="value">The data message to pass</param>
    public void OnNext(T value)
    {
        Exception? error = null;

        PruneObserverUnsubscriberList(observerUnsubscriberList);

        foreach (var observerUnsubscriber in observerUnsubscriberList)
        {
            try
            {
                var observer = observerUnsubscriber.ObserverWeakReference.Target as IObserver<T>;
                if (observer != null)
                {
                    observer.OnNext(value);
                }
            }
            catch (Exception exception)
            {
                error = exception;
            }
        }

        if (error != null)
        {
            throw error;
        }
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

    /// <summary>
    /// Prunes the list from dead observer s and unsubscribers
    /// </summary>
    /// <param name="observerUnsubscriberList"></param>
    private static void PruneObserverUnsubscriberList(
        ICollection<ObserverUnsubscriberStruct> observerUnsubscriberList
    )
    {
        var deadItems = observerUnsubscriberList
            .Where(u => !u.ObserverWeakReference.IsAlive || !u.UnsubscriberWeakReference.IsAlive)
            .ToList();

        deadItems.ForEach(u => observerUnsubscriberList.Remove(u));
    }
}
