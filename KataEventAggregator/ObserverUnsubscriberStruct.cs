#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// Data structure to manage references of observer/unsubscriber instances.
/// </summary>
internal struct ObserverUnsubscriberStruct
{
    public object? ObserverReference;
    public WeakReference ObserverWeakReference;
    public WeakReference UnsubscriberWeakReference;
}
