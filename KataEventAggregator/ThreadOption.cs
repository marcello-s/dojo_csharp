#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// Specifies on which thread a subscriber will be called.
/// </summary>
public enum ThreadOption
{
    /// <summary>
    /// The call is done on the same thread on which was published.
    /// </summary>
    PublisherThread,

    /// <summary>
    /// The call is done on the UI thread.
    /// </summary>
    UiThread,

    /// <summary>
    /// The call is done asynchronously on a background thread.
    /// </summary>
    BackgroundThread,
}
