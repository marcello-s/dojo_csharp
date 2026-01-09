#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// A reference of this class is handed out for the observer to remove the subscription
/// </summary>
internal class Unsubscriber : MarshalByRefObject, IDisposable
{
    private readonly List<ObserverUnsubscriberStruct> _observerUnsubscriberList;
    private readonly WeakReference _observerWeakReference;

    /// <summary>
    /// Create instance
    /// </summary>
    /// <param name="observerUnsubscriberList">A list of all observers of the IObservable this unsubscriber was created for</param>
    /// <param name="observerWeakReference">A reference to the observer this unsubscriber was created for</param>
    public Unsubscriber(
        List<ObserverUnsubscriberStruct> observerUnsubscriberList,
        WeakReference observerWeakReference
    )
    {
        _observerUnsubscriberList = observerUnsubscriberList;
        _observerWeakReference = observerWeakReference;
    }

    /// <summary>
    /// Performs application-based tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (_observerWeakReference.Target == null)
            return;
        var itemToRemove = _observerUnsubscriberList.FirstOrDefault(item =>
            item.ObserverWeakReference.Equals(_observerWeakReference)
        );
        _observerUnsubscriberList.Remove(itemToRemove);
    }
}
